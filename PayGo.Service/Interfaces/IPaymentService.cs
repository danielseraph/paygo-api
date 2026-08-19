using PayGo.Core.Responses;
using PayGo.Model.Requests;
using PayGo.Model.Responses;

namespace PayGo.Service.Interfaces;

public interface IPaymentService
{
    Task<ApiResponse<InitializePaymentResponse>> InitializePaymentAsync(InitializePaymentRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<PaymentVerificationResponse>> VerifyPaymentAsync(string reference, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> ProcessWebhookAsync(string payload, string signatureHeader, CancellationToken cancellationToken = default);
}
