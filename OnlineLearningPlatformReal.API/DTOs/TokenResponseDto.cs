namespace OnlineLearningPlatform.API.DTOs
{
    public class TokenResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;

        public object User { get; set; } = new { };
    }
}
