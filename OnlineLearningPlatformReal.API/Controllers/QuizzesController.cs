using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.API.Data;
using OnlineLearningPlatform.API.DTOs.Quizzes;
using OnlineLearningPlatform.API.Models;
using System.Security.Claims;

namespace OnlineLearningPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizzesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuizzesController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> CreateQuiz(QuizCreateDto dto)
        {
            int instructorId = GetUserId();

            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == dto.CourseId && c.CreatedBy == instructorId);

            if (course == null)
                return Forbid();

            var lessonExists = await _context.Lessons
                .AnyAsync(l => l.Id == dto.LessonId && l.CourseId == dto.CourseId);

            if (!lessonExists)
                return BadRequest(new { message = "Lesson does not belong to this course" });

            var quiz = new Quiz
            {
                CourseId = dto.CourseId,
                LessonId = dto.LessonId,
                Title = dto.Title,
                PassingScore = dto.PassingScore,
                TimeLimit = dto.TimeLimit
            };

            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Quiz created successfully",
                quizId = quiz.Id
            });
        }
        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetQuizzesByCourse(int courseId)
        {
            var quizzes = await _context.Quizzes
                .Where(q => q.CourseId == courseId)
                .Select(q => new
                {
                    q.Id,
                    q.Title,
                    q.PassingScore,
                    q.TimeLimit
                })
                .ToListAsync();

            return Ok(quizzes);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuizById(int id)
        {
            var quiz = await _context.Quizzes
                .Where(q => q.Id == id)
                .Select(q => new
                {
                    q.Id,
                    q.Title,
                    q.PassingScore,
                    q.TimeLimit,
                    q.CourseId,
                    q.LessonId
                })
                .FirstOrDefaultAsync();

            if (quiz == null)
                return NotFound();

            return Ok(quiz);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> UpdateQuiz(int id, QuizUpdateDto dto)
        {
            int instructorId = GetUserId();

            var quiz = await _context.Quizzes
                .Include(q => q.Course)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null)
                return NotFound();

            if (quiz.Course.CreatedBy != instructorId)
                return Forbid();

            quiz.Title = dto.Title;
            quiz.PassingScore = dto.PassingScore;
            quiz.TimeLimit = dto.TimeLimit;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Quiz updated successfully" });
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> DeleteQuiz(int id)
        {
            int instructorId = GetUserId();

            var quiz = await _context.Quizzes
                .Include(q => q.Course)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null)
                return NotFound();

            if (quiz.Course.CreatedBy != instructorId)
                return Forbid();

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Quiz deleted successfully" });
        }
    }
}
