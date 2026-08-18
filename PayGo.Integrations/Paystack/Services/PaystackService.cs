using Microsoft.Extensions.Logging;
using PayGo.Core.Config;
using PayGo.Integrations.Paystack.Abstractions;
using PayGo.Integrations.Paystack.DTOs;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
namespace PayGo.Integrations.Paystack.Services;

public class PaystackService : IPaystackService
{
    private readonly HttpClient _httpClient;
    private readonly PaystackSettings _settings;
    private readonly ILogger<PaystackService> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    public PaystackService(HttpClient httpClient, PaystackSettings settings, ILogger<PaystackService> logger)
    {
        _httpClient = httpClient;
        _settings = settings;
        _logger = logger;
    }
    public async Task<PaystackInitializeResponse> InitializeTransactionAsync(PaystackInitializeRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Initializing Paystack transaction for reference: {Reference}", request.Reference);
        var json = JsonSerializer.Serialize(request, JsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("transaction/initialize", content, cancellationToken);
        var responsBody = await response.Content.ReadAsStringAsync(cancellationToken);
        
        if(!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to initialize Paystack transaction. Status Code: {StatusCode}, Response: {Response}", response.StatusCode, responsBody);
            throw new HttpRequestException($"Failed to initialize Paystack transaction. Status Code: {response.StatusCode}, Response: {responsBody}");
        }
        var result = JsonSerializer.Deserialize<PaystackInitializeResponse>(responsBody, JsonOptions);
        return result ?? throw new InvalidOperationException("Failed to deserialize Paystack initialize response.");
    }

    public async Task<PaystackVerifyResponse> VerifyTransactionAsync(string reference, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Verifying Paystack transaction for reference: {Reference}", reference);
        var response = await _httpClient.GetAsync($"/transaction/verify/{reference}", cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Paystack verify failed. Reference: {Reference}, Status: {StatusCode}, Body: {Body}",
                reference, response.StatusCode, responseBody);
            throw new HttpRequestException(
                $"Paystack transaction verification failed: {responseBody}",
                null,
                response.StatusCode);
        }
        var result = JsonSerializer.Deserialize<PaystackVerifyResponse>(responseBody, JsonOptions);
        return result ?? throw new InvalidOperationException("Failed to deserialize Paystack verify response.");

    }


    public async Task<bool> ValidateWebhookRequestAsync(string payload, string paystackSignatureHeader)
    {
        if (string.IsNullOrWhiteSpace(_settings.WebhookSecret))
        {
            _logger.LogWarning("Paystack WebhookSecret is not configured. Signature validation skipped.");
            return false;
        }
        // Paystack signs the raw payload with HMAC-SHA512 using your secret key
        var secretBytes = Encoding.UTF8.GetBytes(_settings.WebhookSecret);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        var computedHash = HMACSHA512.HashData(secretBytes, payloadBytes);
        var computedSignature = Convert.ToHexString(computedHash).ToLowerInvariant();
        // Use CryptographicOperations.FixedTimeEquals to prevent timing attacks
        var computedBytes = Encoding.UTF8.GetBytes(computedSignature);
        var headerBytes = Encoding.UTF8.GetBytes(paystackSignatureHeader.ToLowerInvariant());
        if (computedBytes.Length != headerBytes.Length)
            return false;
        return CryptographicOperations.FixedTimeEquals(computedBytes, headerBytes);
    }
}
