using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.API.Data;
using OnlineLearningPlatform.API.DTOs;
using OnlineLearningPlatform.API.Models;
using System.IdentityModel.Tokens.Jwt;

namespace OnlineLearningPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Instructor")]
        [HttpPost]
        public async Task<IActionResult> CreateCourse(CourseCreateDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
             ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub));


            var course = new Course
            {
                Title = dto.Title,
                ShortDescription = dto.ShortDescription,
                LongDescription = dto.LongDescription,
                Category = dto.Category,
                Difficulty = dto.Difficulty,
                Thumbnail = dto.Thumbnail,
                IsPublished = dto.IsPublished,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Course created successfully",
                courseId = course.Id
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPublishedCourses()
        {
            var courses = await _context.Courses
                .Where(c => c.IsPublished)
                .Select(c => new CourseResponseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    ShortDescription = c.ShortDescription,
                    Category = c.Category,
                    Difficulty = c.Difficulty,
                    Thumbnail = c.Thumbnail,
                    IsPublished = c.IsPublished,
                    CreatedBy = c.CreatedBy,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return Ok(courses);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var course = await _context.Courses
                .Where(c => c.Id == id && c.IsPublished)
                .Select(c => new CourseResponseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    ShortDescription = c.ShortDescription,
                    Category = c.Category,
                    Difficulty = c.Difficulty,
                    Thumbnail = c.Thumbnail,
                    IsPublished = c.IsPublished,
                    CreatedBy = c.CreatedBy,
                    CreatedAt = c.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (course == null)
                return NotFound(new { message = "Course not found" });

            return Ok(course);
        }
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> UpdateCourse(int id, CourseUpdateDto dto)
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized(new { message = "Invalid token" });

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
            if (course == null)
                return NotFound(new { message = "Course not found" });

            if (course.CreatedBy != userId)
                return Forbid();

            course.Title = dto.Title;
            course.ShortDescription = dto.ShortDescription;
            course.LongDescription = dto.LongDescription;
            course.Category = dto.Category;
            course.Difficulty = dto.Difficulty;
            course.Thumbnail = dto.Thumbnail;
            course.IsPublished = dto.IsPublished;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Course updated successfully" });
        }
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized(new { message = "Invalid token" });

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
            if (course == null)
                return NotFound(new { message = "Course not found" });

            if (course.CreatedBy != userId)
                return Forbid();

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Course deleted successfully" });
        }


        private bool TryGetUserId(out int userId)
        {
            userId = 0;

            var idValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            return int.TryParse(idValue, out userId);
        }
    }
}
    
