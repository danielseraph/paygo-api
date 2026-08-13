using System;
using System.Collections.Generic;
using System.Text;

namespace PayGo.Core.Enums
{
    // Represents the status of a payment
    public enum PaymentStatus
    {
        Pending = 1,
        Success = 2,
        Failed = 3,
        Refunded = 4,
        PartiallyRefunded = 5,
        Abandoned = 6,
    }
}
