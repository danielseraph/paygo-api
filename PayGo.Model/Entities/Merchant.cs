using PayGo.Core.Enums;
using PayGo.Model.Entities.Common;
using System.Transactions;

namespace PayGo.Model.Entities
{
    // Represents a merchant entity
    public class Merchant : BaseEntity
    {
        public string BusinessName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? BusinessAddress { get; set; }
        public string? LogoUrl { get; set; }
        public string? WebsiteUrl { get; set; }
        public MerchantStatus Status { get; set; } = MerchantStatus.PendingVerification;
        public string CurrencyCode { get; set; } = "NGN";
        // API credentials
        public string? ApiKey { get; set; }
        public string? ApiSecret { get; set; }
        // Provider configuration
        public string? PaystackSubaccountCode { get; set; }
        // Navigation properties
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<PaymentLink> PaymentLinks { get; set; } = new List<PaymentLink>();
        public ICollection<LedgerEntry> LedgerEntries { get; set; } = new List<LedgerEntry>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
