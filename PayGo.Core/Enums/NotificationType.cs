namespace PayGo.Core.Enums
{
    //Categorises merchant-facing notifications.
    public enum NotificationType
    {
        PaymentReceived = 1,
        PaymentFailed = 2,
        RefundProcessed = 3,
        SuspiciousTransaction = 4,
        ReconciliationException = 5,
        SettlementReceived = 6,
    }
}
