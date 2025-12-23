using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.API.DTOs.Users
{
    public class UpdateUserDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
    }
}
