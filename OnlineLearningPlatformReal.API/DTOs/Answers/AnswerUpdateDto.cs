using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.API.DTOs.Answers
{
    public class AnswerUpdateDto
    {
        [Required]
        public string AnswerText { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }
    }
}
