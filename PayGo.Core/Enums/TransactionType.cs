namespace PayGo.Core.Enums
{
    // Represents the type of transaction
    public enum TransactionType
    {
        Payment = 1,
        Refund = 2,
        PartialRefund = 3,
        Chargeback = 4,
        Settlement = 5,
        Fee = 6,
    }
}
