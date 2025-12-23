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
public class LessonCompletionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LessonCompletionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Student: get my completions
    [HttpGet("my")]
    public async Task<IActionResult> My()
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
        );

        return Ok(await _context.LessonCompletions
            .Where(l => l.UserId == userId)
            .ToListAsync());
    }

    // Admin: delete
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var lc = await _context.LessonCompletions.FindAsync(id);
        if (lc == null) return NotFound();

        _context.LessonCompletions.Remove(lc);
        await _context.SaveChangesAsync();

        return Ok("Lesson completion removed");
    }
}
