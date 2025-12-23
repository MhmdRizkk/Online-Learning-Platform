using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.API.DTOs.Questions
{
    public class QuestionCreateDto
    {
        [Required]
        public int QuizId { get; set; }

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        [Required]
        public string QuestionType { get; set; } = string.Empty;
    }
}
