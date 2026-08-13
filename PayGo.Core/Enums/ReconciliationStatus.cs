namespace PayGo.Core.Enums
{
    // Represents the reconciliation status
    public enum ReconciliationStatus
    {
        Pending = 1,
        Matched = 2,
        Mismatched = 3,
        Missing = 4,
        Exception = 5
    }
}
