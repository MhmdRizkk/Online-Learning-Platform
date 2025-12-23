using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.API.Data;
using OnlineLearningPlatform.API.DTOs;
using OnlineLearningPlatform.API.Models;
using OnlineLearningPlatform.API.Services;

namespace OnlineLearningPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly JwtService _jwtService;
        private readonly RefreshTokenService _refreshTokenService;

        public AuthController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            JwtService jwtService,
            RefreshTokenService refreshTokenService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // ModelState is automatically validated because of [ApiController]
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null)
                return BadRequest(new { message = "Email already exists." });

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                UserName = dto.Email,
                CreatedAt = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
                return BadRequest(new { message = "Registration failed.", errors = createResult.Errors });

            // Ensure roles exist
            var role = string.IsNullOrWhiteSpace(dto.Role) ? "User" : dto.Role;
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole<int>(role));

            await _userManager.AddToRoleAsync(user, role);

            return Ok(new { message = "User registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password." });

            var valid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!valid)
                return Unauthorized(new { message = "Invalid email or password." });

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _jwtService.GenerateAccessToken(user, roles);

            // Create refresh token (rotate on every use)
            var refreshTokenPlain = _refreshTokenService.GenerateRefreshTokenPlain();
            var refreshTokenHash = _refreshTokenService.HashToken(refreshTokenPlain);

            _context.RefreshTokens.Add(new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshTokenHash,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(14)
            });

            await _context.SaveChangesAsync();

            return Ok(new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenPlain,
                User = new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    roles
                }
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
        {
            var tokenHash = _refreshTokenService.HashToken(dto.RefreshToken);

            var stored = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

            if (stored == null || !stored.IsActive)
                return Unauthorized(new { message = "Invalid or expired refresh token." });

            // Rotate refresh token
            var newRefreshPlain = _refreshTokenService.GenerateRefreshTokenPlain();
            var newRefreshHash = _refreshTokenService.HashToken(newRefreshPlain);

            stored.RevokedAtUtc = DateTime.UtcNow;
            stored.ReplacedByTokenHash = newRefreshHash;

            _context.RefreshTokens.Add(new RefreshToken
            {
                UserId = stored.UserId,
                TokenHash = newRefreshHash,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(14)
            });

            var roles = await _userManager.GetRolesAsync(stored.User);
            var newAccess = _jwtService.GenerateAccessToken(stored.User, roles);

            await _context.SaveChangesAsync();

            return Ok(new TokenResponseDto
            {
                AccessToken = newAccess,
                RefreshToken = newRefreshPlain,
                User = new
                {
                    stored.User.Id,
                    stored.User.FullName,
                    stored.User.Email,
                    roles
                }
            });
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] RevokeTokenDto dto)
        {
            var tokenHash = _refreshTokenService.HashToken(dto.RefreshToken);

            var stored = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);
            if (stored == null)
                return NotFound(new { message = "Refresh token not found." });

            if (stored.IsRevoked)
                return Ok(new { message = "Token already revoked." });

            stored.RevokedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Token revoked." });
        }
    }
}
