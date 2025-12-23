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
public class QuizAttemptsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public QuizAttemptsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Student: my attempts
    [HttpGet("my")]
    public async Task<IActionResult> My()
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
        );

        return Ok(await _context.QuizAttempts
            .Where(q => q.UserId == userId)
            .ToListAsync());
    }

    // Admin: get attempt by id
    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var attempt = await _context.QuizAttempts.FindAsync(id);
        return attempt == null ? NotFound() : Ok(attempt);
    }

    // Admin: delete
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var attempt = await _context.QuizAttempts.FindAsync(id);
        if (attempt == null) return NotFound();

        _context.QuizAttempts.Remove(attempt);
        await _context.SaveChangesAsync();

        return Ok("Quiz attempt deleted");
    }
}
