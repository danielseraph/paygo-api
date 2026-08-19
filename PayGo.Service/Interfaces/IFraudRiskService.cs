using PayGo.Core.Enums;
using PayGo.Core.Responses;

namespace PayGo.Service.Interfaces;

public interface IFraudRiskService
{
    Task<ApiResponse<RiskLevel>> EvaluateTransactionRiskAsync(Guid merchantId, decimal amount, string email, CancellationToken cancellationToken = default);
}
