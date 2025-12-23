using Microsoft.AspNetCore.Identity;

namespace OnlineLearningPlatform.API.Models
{

    public class User : IdentityUser<int>
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Course> CoursesCreated { get; set; } = new List<Course>();
        public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
        public ICollection<LessonCompletion> LessonCompletions { get; set; } = new List<LessonCompletion>();
        public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
