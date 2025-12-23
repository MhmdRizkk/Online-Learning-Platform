using System.Security.Cryptography;
using System.Text;

namespace OnlineLearningPlatform.API.Services
{
    public class RefreshTokenService
    {
        // Create a cryptographically strong random token (base64url)
        public string GenerateRefreshTokenPlain()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Base64UrlEncode(bytes);
        }

        public string HashToken(string tokenPlain)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(tokenPlain);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash); // uppercase hex
        }

        private static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }
    }
}
