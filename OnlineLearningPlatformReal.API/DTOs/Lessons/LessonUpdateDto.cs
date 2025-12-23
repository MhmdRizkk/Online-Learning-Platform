using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.API.DTOs.Lessons
{
    public class LessonUpdateDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public int Order { get; set; }
        public int EstimatedDuration { get; set; }
    }
}
