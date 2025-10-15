using System;
using System.Collections.Generic;

namespace EventPlatform.Domain.Entities;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public Guid RegistrationId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public string? PaymentGateway { get; set; }

    public string? GatewayTransactionId { get; set; }

    public DateTime? TransactionDate { get; set; }

    public virtual Registration Registration { get; set; } = null!;
}
