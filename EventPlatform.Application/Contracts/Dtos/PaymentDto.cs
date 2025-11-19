using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EventPlatform.Application.Contracts.Dtos
{
    public class SepayQrResponse
    {
        public string QrCodeUrl { get; set; }
        public string TransactionDescription { get; set; }
        public decimal Amount { get; set; } // Đã có Amount
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
    }

    public class SepayWebhookPayload
    {
        [JsonPropertyName("id")]
        public string SepayTransactionId { get; set; }

        [JsonPropertyName("reference_number")]
        public string ReferenceNumber { get; set; }

        [JsonPropertyName("transaction_content")]
        public string TransactionContent { get; set; }

        [JsonPropertyName("amount_in")]
        public string AmountIn { get; set; } // Sepay gửi về dạng chuỗi

        [JsonPropertyName("transaction_date")]
        public string TransactionDate { get; set; }

        // Thêm các trường khác nếu bạn cần
    }
}
