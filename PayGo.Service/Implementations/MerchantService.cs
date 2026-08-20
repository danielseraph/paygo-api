using Microsoft.EntityFrameworkCore;
using PayGo.Core.Enums;
using PayGo.Core.Responses;
using PayGo.Model.Entities;
using PayGo.Model.Requests;
using PayGo.Model.Responses;
using PayGo.Persistence;
using PayGo.Service.Interfaces;
using System.Security.Cryptography;

namespace PayGo.Service.Implementations;

public class MerchantService : IMerchantService
{
    private readonly PayGoDbContext _dbContext;

    public MerchantService(PayGoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<MerchantResponse>> CreateMerchantAsync(CreateMerchantRequest request, CancellationToken cancellationToken = default)
    {
        // Check if a merchant with the same email already exists
        var existing = await _dbContext.Merchants
            .FirstOrDefaultAsync(m => m.Email == request.Email, cancellationToken);

        if (existing != null)
        {
            return new ApiResponse<MerchantResponse>().FailureResponse($"Merchant with email {request.Email} already exists.");
        }

        // Generate unique API key and secret for the merchant
        var rawApiKey = $"pk_live_{Guid.NewGuid().ToString("N")}";
        var rawApiSecret = $"sk_live_{Guid.NewGuid().ToString("N")}";

        // Hash the API secret before storing it in the database
        var hashedApiSecret = Convert.ToHexString(
            SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(rawApiSecret))
            ).ToLowerInvariant();

        // Create a new merchant entity
        var merchant = new Merchant
        {
            BusinessName = request.BusinessName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            BusinessAddress = request.BusinessAddress,
            CurrencyCode = request.CurrencyCode,
            Status = MerchantStatus.Active,
            ApiKey = rawApiKey,
            ApiSecret = hashedApiSecret,
        };

        _dbContext.Merchants.Add(merchant);
        await _dbContext.SaveChangesAsync(cancellationToken);
        // Map response — and inject the raw secret ONLY here, ONLY at creation time
        var response = MapToResponse(merchant);
        response.ApiSecret = rawApiSecret; // raw value — merchant must save this immediately
        return new ApiResponse<MerchantResponse>().SuccessResponse(response, "Merchant created successfully. Store your API secret now — it will not be shown again.", 201);
    }

    public async Task<ApiResponse<MerchantResponse>> GetMerchantByIdAsync(Guid merchantId, CancellationToken cancellationToken = default)
    {
        var merchant = await _dbContext.Merchants
            .FirstOrDefaultAsync(m => m.Id == merchantId, cancellationToken);

        if (merchant == null)
        {
            return new ApiResponse<MerchantResponse>().FailureResponse("Merchant not found", 404);
        }

        return new ApiResponse<MerchantResponse>().SuccessResponse(MapToResponse(merchant));
    }

    public async Task<ApiResponse<MerchantResponse>> GetMerchantByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var merchant = await _dbContext.Merchants
            .FirstOrDefaultAsync(m => m.Email == email, cancellationToken);

        if (merchant == null)
        {
            return new ApiResponse<MerchantResponse>().FailureResponse("Merchant not found", 404);
        }

        return new ApiResponse<MerchantResponse>().SuccessResponse(MapToResponse(merchant));
    }

    private static MerchantResponse MapToResponse(Merchant merchant)
    {
        return new MerchantResponse
        {
            Id = merchant.Id,
            BusinessName = merchant.BusinessName,
            Email = merchant.Email,
            PhoneNumber = merchant.PhoneNumber,
            Status = merchant.Status,
            CurrencyCode = merchant.CurrencyCode,
            ApiKey = merchant.ApiKey,
            CreatedAt = merchant.CreatedAt
        };
    }
}
