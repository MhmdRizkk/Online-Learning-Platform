using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.API.DTOs
{
    public class RevokeTokenDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
