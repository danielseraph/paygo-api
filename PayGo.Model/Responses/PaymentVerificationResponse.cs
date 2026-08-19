using PayGo.Core.Enums;

namespace PayGo.Model.Responses;

public class PaymentVerificationResponse
{
    public string Reference { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "NGN";
    public string GatewayResponse { get; set; } = string.Empty;
    public DateTime? PaidAt { get; set; }
    public string Channel { get; set; } = string.Empty;
    public Guid MerchantId { get; set; }
}
