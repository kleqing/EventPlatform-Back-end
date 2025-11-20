using System;

namespace EventPlatform.Application.Contracts.Dtos
{
    public class AdminTransactionDto
    {
        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string? PaymentGateway { get; set; }
        public string? GatewayTransactionId { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string BuyerName { get; set; } = string.Empty;
        public string BuyerEmail { get; set; } = string.Empty;
        public string EventTitle { get; set; } = string.Empty;
    }
}
