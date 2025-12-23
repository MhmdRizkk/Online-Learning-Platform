using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.API.Data;
using OnlineLearningPlatform.API.Models;

namespace OnlineLearningPlatform.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/analytics")]
    public class AdminAnalyticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public AdminAnalyticsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("top-courses")]
        public async Task<IActionResult> GetTopCourses()
        {
            var courses = await _context.Courses
                .Select(c => new
                {
                    c.Id,
                    c.Title,
                    lessonCompletions = _context.LessonCompletions
                        .Count(lc =>
                            _context.Lessons
                                .Where(l => l.CourseId == c.Id)
                                .Select(l => l.Id)
                                .Contains(lc.LessonId)
                        ),
                    quizAttempts = _context.QuizAttempts
                        .Count(qa => qa.Quiz.CourseId == c.Id)
                })
                .OrderByDescending(c => c.lessonCompletions + c.quizAttempts)
                .Take(5)
                .ToListAsync();

            return Ok(courses);
        }

        [HttpGet("instructors")]
        public async Task<IActionResult> GetInstructorStats()
        {
            var instructors = new List<object>();
            var users = await _userManager.GetUsersInRoleAsync("Instructor");

            foreach (var i in users)
            {
                instructors.Add(new
                {
                    instructorId = i.Id,
                    i.FullName,
                    coursesCreated = _context.Courses.Count(c => c.CreatedBy == i.Id),
                    certificatesIssued = _context.Certificates.Count(c => c.Course.CreatedBy == i.Id)
                });
            }

            return Ok(instructors);
        }

        [HttpGet("students")]
        public async Task<IActionResult> GetStudentStats()
        {
            var students = new List<object>();
            var users = await _userManager.GetUsersInRoleAsync("Student");

            foreach (var s in users)
            {
                students.Add(new
                {
                    studentId = s.Id,
                    s.FullName,
                    lessonsCompleted = _context.LessonCompletions.Count(lc => lc.UserId == s.Id),
                    quizzesAttempted = _context.QuizAttempts.Count(qa => qa.UserId == s.Id),
                    certificatesEarned = _context.Certificates.Count(c => c.UserId == s.Id)
                });
            }

            return Ok(students);
        }
    }
}
