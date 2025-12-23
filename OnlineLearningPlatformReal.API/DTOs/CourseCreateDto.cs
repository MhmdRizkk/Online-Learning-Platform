using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.API.DTOs
{
    public class CourseCreateDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MinLength(10)]
        [MaxLength(500)]
        public string ShortDescription { get; set; } = string.Empty;

        [Required]
        [MinLength(10)]
        public string LongDescription { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Difficulty { get; set; } = string.Empty;

        [Url]
        public string Thumbnail { get; set; } = string.Empty;

        public bool IsPublished { get; set; }
    }
}
