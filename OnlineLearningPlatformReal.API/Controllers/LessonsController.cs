using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.API.Data;
using OnlineLearningPlatform.API.DTOs.Lessons;
using OnlineLearningPlatform.API.Models;
using System.Security.Claims;

namespace OnlineLearningPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LessonsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LessonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> CreateLesson(LessonCreateDto dto)
        {
            int instructorId = GetUserId();

            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == dto.CourseId && c.CreatedBy == instructorId);

            if (course == null)
                return Forbid();

            var lesson = new Lesson
            {
                CourseId = dto.CourseId,
                Title = dto.Title,
                Content = dto.Content,
                VideoUrl = dto.VideoUrl,
                Order = dto.Order,
                EstimatedDuration = dto.EstimatedDuration
            };

            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Lesson created successfully",
                lessonId = lesson.Id
            });
        }
        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetLessonsByCourse(int courseId)
        {
            var lessons = await _context.Lessons
                .Where(l => l.CourseId == courseId)
                .OrderBy(l => l.Order)
                .Select(l => new
                {
                    l.Id,
                    l.Title,
                    l.Order,
                    l.EstimatedDuration
                })
                .ToListAsync();

            return Ok(lessons);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> UpdateLesson(int id, LessonUpdateDto dto)
        {
            int instructorId = GetUserId();

            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null)
                return NotFound();

            if (lesson.Course.CreatedBy != instructorId)
                return Forbid();

            lesson.Title = dto.Title;
            lesson.Content = dto.Content;
            lesson.VideoUrl = dto.VideoUrl;
            lesson.Order = dto.Order;
            lesson.EstimatedDuration = dto.EstimatedDuration;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Lesson updated successfully" });
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            int instructorId = GetUserId();

            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null)
                return NotFound();

            if (lesson.Course.CreatedBy != instructorId)
                return Forbid();

            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Lesson deleted successfully" });
        }
    }
}
