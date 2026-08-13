namespace PayGo.Core.Constants
{
    // Represents constants used in the PayGo application
    public class PayGoConstants
    {
        public const string DefaultCurrency = "NGN";
        public const int MaxPaymentLinkExpiryDays = 30;
        public const int DefaultPageSize = 25;
        public const int MaxPageSize = 100;
        public const int WebhookMaxRetries = 3;
        public const string PaystackProviderName = "Paystack";
    }
}
