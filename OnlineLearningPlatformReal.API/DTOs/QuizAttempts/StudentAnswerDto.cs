using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.API.DTOs.QuizAttempts
{
    public class StudentAnswerDto
    {
        [Required]
        public int QuestionId { get; set; }

        [Required]
        public int AnswerId { get; set; }
    }
}
