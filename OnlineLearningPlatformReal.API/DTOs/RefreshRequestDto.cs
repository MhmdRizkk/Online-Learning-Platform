using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.API.DTOs
{
    public class RefreshRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
