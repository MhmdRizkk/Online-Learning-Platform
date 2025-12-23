using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.API.DTOs.QuizAttempts
{
    public class QuizAttemptCreateDto
    {
        [Required]
        public int QuizId { get; set; }

        [Required]
        public List<StudentAnswerDto> Answers { get; set; } = new();
    }
}
