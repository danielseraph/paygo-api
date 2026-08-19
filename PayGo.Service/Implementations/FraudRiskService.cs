using Microsoft.EntityFrameworkCore;
using PayGo.Core.Enums;
using PayGo.Core.Responses;
using PayGo.Persistence;
using PayGo.Service.Interfaces;

namespace PayGo.Service.Implementations;

public class FraudRiskService : IFraudRiskService
{
    private readonly PayGoDbContext _dbContext;

    public FraudRiskService(PayGoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<RiskLevel>> EvaluateTransactionRiskAsync(Guid merchantId, decimal amount, string email, CancellationToken cancellationToken = default)
    {
        // Simple rule engine for fraud & risk evaluation:
        // 1. High transaction amount (> 1,000,000 NGN)
        // 2. High velocity (multiple transactions from same email in last 5 minutes)
        
        var fiveMinutesAgo = DateTime.UtcNow.AddMinutes(-5);
        var recentTxCount = await _dbContext.Transactions
            .CountAsync(t => t.CustomerEmail == email && t.CreatedAt >= fiveMinutesAgo, cancellationToken);

        if (amount > 1_000_000m || recentTxCount >= 5)
        {
            return new ApiResponse<RiskLevel>().SuccessResponse(RiskLevel.High, "High risk detected based on velocity or volume rules");
        }

        if (amount > 200_000m || recentTxCount >= 3)
        {
            return new ApiResponse<RiskLevel>().SuccessResponse(RiskLevel.Medium, "Medium risk detected");
        }

        return new ApiResponse<RiskLevel>().SuccessResponse(RiskLevel.Low, "Low risk evaluation");
    }
}
