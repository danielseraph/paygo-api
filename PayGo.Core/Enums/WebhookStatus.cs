namespace PayGo.Core.Enums
{
    // Represents the status of a webhook
    public enum WebhookStatus
    {
        Received = 1,
        Processing = 2,
        Processed = 3,
        Failed = 4,
        Ignored = 5
    }
}
