namespace PayGo.Core.Config
{
    // Represents the settings for JWT authentication
    public class JwtSettings
    {
        public const string SectionName = "Jwt";
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiryInMinutes { get; set; }
        public string RefreshTokenExpiryInDays { get; set; } = string.Empty;
    }
}
