using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.API.Data;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Instructor")]
public class StudentAnswersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public StudentAnswersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _context.StudentAnswers.ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var answer = await _context.StudentAnswers.FindAsync(id);
        return answer == null ? NotFound() : Ok(answer);
    }
}
