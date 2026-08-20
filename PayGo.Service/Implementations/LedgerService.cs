using Microsoft.EntityFrameworkCore;
using PayGo.Core.Enums;
using PayGo.Core.Responses;
using PayGo.Model.Entities;
using PayGo.Model.Requests;
using PayGo.Model.Responses;
using PayGo.Persistence;
using PayGo.Service.Interfaces;
using System.Data;
using System.Data.Common;

namespace PayGo.Service.Implementations;

public class LedgerService : ILedgerService
{
    private readonly PayGoDbContext _dbContext;

    public LedgerService(PayGoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<LedgerEntryResponse>> RecordEntryAsync(RecordLedgerEntryRequest request, CancellationToken cancellationToken = default)
    {
        var merchantExists = await _dbContext.Merchants.AnyAsync(m => m.Id == request.MerchantId, cancellationToken);
        if (!merchantExists)
        {
            return new ApiResponse<LedgerEntryResponse>().FailureResponse("Merchant not found", 404);
        }

        // Open a serializable transaction so that concurrent requests for the same merchant
        // are forced to execute sequentially. This prevents the read-modify-write race condition
        // where two simultaneous payments corrupt the BalanceAfter value.
        await using var dbTransaction = await _dbContext.Database
            .BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

        try
        {
            // Inside the serializable transaction, this read acquires a range lock in PostgreSQL.
            // Any other request trying to read/write ledger entries for this merchant will WAIT
            // until we call CommitAsync below.
            var lastEntry = await _dbContext.LedgerEntries
                .Where(l => l.MerchantId == request.MerchantId)
                .OrderByDescending(l => l.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            decimal currentBalance = lastEntry?.BalanceAfter ?? 0m;

            decimal balanceAfter = request.EntryType switch
            {
                LedgerEntryType.Credit => currentBalance + request.Amount,
                LedgerEntryType.Debit => currentBalance - request.Amount,
                _ => currentBalance
            };

            var entry = new LedgerEntry
            {
                MerchantId = request.MerchantId,
                TransactionId = request.TransactionId,
                EntryType = request.EntryType,
                TransactionType = TransactionType.Payment,
                Amount = request.Amount,
                BalanceAfter = balanceAfter,
                CurrencyCode = request.CurrencyCode ?? "NGN", 
                Description = request.Description,
                Reference = request.Reference
            };

            _dbContext.LedgerEntries.Add(entry);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await dbTransaction.CommitAsync(cancellationToken);

            var response = MapToResponse(entry);
            return new ApiResponse<LedgerEntryResponse>().SuccessResponse(response, "Ledger entry recorded successfully", 201);
        }
        catch
        {
            await dbTransaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
    public async Task<ApiResponse<decimal>> GetMerchantBalanceAsync(Guid merchantId, CancellationToken cancellationToken = default)
    {
        var lastEntry = await _dbContext.LedgerEntries
            .Where(l => l.MerchantId == merchantId)
            .OrderByDescending(l => l.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        decimal balance = lastEntry?.BalanceAfter ?? 0m;
        return new ApiResponse<decimal>().SuccessResponse(balance);
    }

    public async Task<ApiResponse<List<LedgerEntryResponse>>> GetMerchantLedgerEntriesAsync(Guid merchantId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var entries = await _dbContext.LedgerEntries
            .Where(l => l.MerchantId == merchantId)
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => MapToResponse(l))
            .ToListAsync(cancellationToken);

        return new ApiResponse<List<LedgerEntryResponse>>().SuccessResponse(entries);
    }

    private static LedgerEntryResponse MapToResponse(LedgerEntry entry)
    {
        return new LedgerEntryResponse
        {
            Id = entry.Id,
            MerchantId = entry.MerchantId,
            TransactionId = entry.TransactionId,
            Amount = entry.Amount,
            EntryType = entry.EntryType,
            Description = entry.Description ?? string.Empty,
            Reference = entry.Reference ?? string.Empty,
            BalanceAfter = entry.BalanceAfter,
            CreatedAt = entry.CreatedAt
        };
    }
}
