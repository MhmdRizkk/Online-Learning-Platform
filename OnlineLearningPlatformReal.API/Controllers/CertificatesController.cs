using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.API.Data;
using OnlineLearningPlatform.API.Models;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CertificatesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CertificatesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Admin: get all
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _context.Certificates.ToListAsync());

    // Admin: get by id
    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var cert = await _context.Certificates.FindAsync(id);
        return cert == null ? NotFound() : Ok(cert);
    }

    // Student: get my certificates
    [HttpGet("my")]
    public async Task<IActionResult> GetMy()
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
        );

        return Ok(await _context.Certificates
            .Where(c => c.UserId == userId)
            .ToListAsync());
    }

    // Admin: delete
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cert = await _context.Certificates.FindAsync(id);
        if (cert == null) return NotFound();

        _context.Certificates.Remove(cert);
        await _context.SaveChangesAsync();

        return Ok("Certificate deleted");
    }
}
