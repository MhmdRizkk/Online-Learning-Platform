using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.API.Data;
using OnlineLearningPlatform.API.DTOs.Questions;
using OnlineLearningPlatform.API.Models;
using System.Security.Claims;

namespace OnlineLearningPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuestionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> CreateQuestion(QuestionCreateDto dto)
        {
            int instructorId = GetUserId();

            var quiz = await _context.Quizzes
                .Include(q => q.Course)
                .FirstOrDefaultAsync(q => q.Id == dto.QuizId);

            if (quiz == null)
                return NotFound(new { message = "Quiz not found" });

            if (quiz.Course.CreatedBy != instructorId)
                return Forbid();

            var question = new Question
            {
                QuizId = dto.QuizId,
                QuestionText = dto.QuestionText,
                QuestionType = dto.QuestionType
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Question created successfully",
                questionId = question.Id
            });
        }
        [HttpGet("quiz/{quizId}")]
        public async Task<IActionResult> GetQuestionsByQuiz(int quizId)
        {
            var questions = await _context.Questions
                .Where(q => q.QuizId == quizId)
                .Select(q => new
                {
                    q.Id,
                    q.QuestionText,
                    q.QuestionType
                })
                .ToListAsync();

            return Ok(questions);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestionById(int id)
        {
            var question = await _context.Questions
                .Where(q => q.Id == id)
                .Select(q => new
                {
                    q.Id,
                    q.QuestionText,
                    q.QuestionType,
                    q.QuizId
                })
                .FirstOrDefaultAsync();

            if (question == null)
                return NotFound();

            return Ok(question);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> UpdateQuestion(int id, QuestionUpdateDto dto)
        {
            int instructorId = GetUserId();

            var question = await _context.Questions
                .Include(q => q.Quiz)
                .ThenInclude(qz => qz.Course)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
                return NotFound();

            if (question.Quiz.Course.CreatedBy != instructorId)
                return Forbid();

            question.QuestionText = dto.QuestionText;
            question.QuestionType = dto.QuestionType;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Question updated successfully" });
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            int instructorId = GetUserId();

            var question = await _context.Questions
                .Include(q => q.Quiz)
                .ThenInclude(qz => qz.Course)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
                return NotFound();

            if (question.Quiz.Course.CreatedBy != instructorId)
                return Forbid();

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Question deleted successfully" });
        }
    }
}
