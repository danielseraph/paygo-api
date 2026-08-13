namespace PayGo.Core.Helpers
{
    public static class IdGenerator
    {
        /// <summary>
        /// Generates a reference like "PG-TXN-20260809-A3F7B2".
        /// </summary>
        public static string GenerateTransactionReference()
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomPart = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
            return $"PG-TXN-{datePart}-{randomPart}";
        }
        /// <summary>
        /// Generates a reference like "PG-PAY-20260809-B1C9D4".
        /// </summary>
        public static string GeneratePaymentLinkReference()
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomPart = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
            return $"PG-PAY-{datePart}-{randomPart}";
        }
        /// <summary>
        /// Generates a reference like "PG-WH-20260809-E5F3A1".
        /// </summary>
        public static string GenerateWebhookReference()
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomPart = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
            return $"PG-WH-{datePart}-{randomPart}";
        }
    }
}
