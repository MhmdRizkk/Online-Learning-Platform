using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.API.Data;
using OnlineLearningPlatform.API.DTOs.Answers;
using OnlineLearningPlatform.API.Models;
using System.Security.Claims;

namespace OnlineLearningPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnswersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AnswersController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> CreateAnswer(AnswerCreateDto dto)
        {
            int instructorId = GetUserId();

            var question = await _context.Questions
                .Include(q => q.Quiz)
                .ThenInclude(qz => qz.Course)
                .FirstOrDefaultAsync(q => q.Id == dto.QuestionId);

            if (question == null)
                return NotFound(new { message = "Question not found" });

            if (question.Quiz.Course.CreatedBy != instructorId)
                return Forbid();

            var answer = new Answer
            {
                QuestionId = dto.QuestionId,
                AnswerText = dto.AnswerText,
                IsCorrect = dto.IsCorrect
            };

            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Answer created successfully",
                answerId = answer.Id
            });
        }
        [HttpGet("question/{questionId}")]
        public async Task<IActionResult> GetAnswersByQuestion(int questionId)
        {
            var answers = await _context.Answers
                .Where(a => a.QuestionId == questionId)
                .Select(a => new
                {
                    a.Id,
                    a.AnswerText,
                    a.IsCorrect
                })
                .ToListAsync();

            return Ok(answers);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnswerById(int id)
        {
            var answer = await _context.Answers
                .Where(a => a.Id == id)
                .Select(a => new
                {
                    a.Id,
                    a.AnswerText,
                    a.IsCorrect,
                    a.QuestionId
                })
                .FirstOrDefaultAsync();

            if (answer == null)
                return NotFound();

            return Ok(answer);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> UpdateAnswer(int id, AnswerUpdateDto dto)
        {
            int instructorId = GetUserId();

            var answer = await _context.Answers
                .Include(a => a.Question)
                .ThenInclude(q => q.Quiz)
                .ThenInclude(qz => qz.Course)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (answer == null)
                return NotFound();

            if (answer.Question.Quiz.Course.CreatedBy != instructorId)
                return Forbid();

            answer.AnswerText = dto.AnswerText;
            answer.IsCorrect = dto.IsCorrect;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Answer updated successfully" });
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> DeleteAnswer(int id)
        {
            int instructorId = GetUserId();

            var answer = await _context.Answers
                .Include(a => a.Question)
                .ThenInclude(q => q.Quiz)
                .ThenInclude(qz => qz.Course)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (answer == null)
                return NotFound();

            if (answer.Question.Quiz.Course.CreatedBy != instructorId)
                return Forbid();

            _context.Answers.Remove(answer);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Answer deleted successfully" });
        }
    }
}
