namespace PayGo.Core.Config
{
    // Represents the settings for Paystack payment provider
    public class PaystackSettings
    {
        public const string SectionName = "PaystackSettings";
        public string SecretKey { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string WebhookSecret { get; set; } = string.Empty;
    }
}
