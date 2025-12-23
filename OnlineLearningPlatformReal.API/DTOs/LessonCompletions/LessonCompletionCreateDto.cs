using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.API.DTOs.LessonCompletions
{
    public class LessonCompletionCreateDto
    {
        [Required]
        public int LessonId { get; set; }
    }
}
