using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.API.Data;
using System.Security.Claims;

namespace OnlineLearningPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProgressController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProgressController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
        [HttpGet("course/{courseId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetCourseProgress(int courseId)
        {
            int studentId = GetUserId();

            int totalLessons = await _context.Lessons
                .CountAsync(l => l.CourseId == courseId);

            if (totalLessons == 0)
            {
                return Ok(new
                {
                    courseId,
                    progress = 0
                });
            }

            int completedLessons = await _context.LessonCompletions
                .CountAsync(lc =>
                    lc.UserId == studentId &&
                    _context.Lessons
                        .Where(l => l.CourseId == courseId)
                        .Select(l => l.Id)
                        .Contains(lc.LessonId)
                );

            int progressPercent = (int)
                Math.Round((double)completedLessons / totalLessons * 100);

            return Ok(new
            {
                courseId,
                completedLessons,
                totalLessons,
                progress = progressPercent
            });
        }
        [HttpGet("my-courses")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyCoursesProgress()
        {
            int studentId = GetUserId();

            var courses = await _context.Courses
                .Include(c => c.Lessons)
                .ToListAsync();

            var result = new List<object>();

            foreach (var course in courses)
            {
                int totalLessons = course.Lessons.Count;

                if (totalLessons == 0)
                    continue;

                int completedLessons = await _context.LessonCompletions
                    .CountAsync(lc =>
                        lc.UserId == studentId &&
                        course.Lessons.Select(l => l.Id)
                            .Contains(lc.LessonId)
                    );

                int progress = (int)
                    Math.Round((double)completedLessons / totalLessons * 100);

                result.Add(new
                {
                    courseId = course.Id,
                    course.Title,
                    completedLessons,
                    totalLessons,
                    progress
                });
            }

            return Ok(result);
        }
    }
}
