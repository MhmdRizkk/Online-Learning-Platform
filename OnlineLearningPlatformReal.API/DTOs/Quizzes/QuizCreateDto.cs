using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.API.DTOs.Quizzes
{
    public class QuizCreateDto
    {
        [Required]
        public int CourseId { get; set; }

        [Required]
        public int LessonId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public int PassingScore { get; set; }
        public int TimeLimit { get; set; }
    }
}
