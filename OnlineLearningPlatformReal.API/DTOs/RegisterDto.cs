using System.ComponentModel.DataAnnotations;

namespace OnlineLearningPlatform.API.DTOs
{
    public class RegisterDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        // Allowed: "Admin" or "User" (default: User)
        [RegularExpression("^(Admin|User)$", ErrorMessage = "Role must be Admin or User.")]
        public string Role { get; set; } = "User";
    }
}
