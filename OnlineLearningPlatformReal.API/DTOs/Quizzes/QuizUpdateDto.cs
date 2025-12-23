using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.API.DTOs.Quizzes
{
    public class QuizUpdateDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        public int PassingScore { get; set; }
        public int TimeLimit { get; set; }
    }
}
