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
        public long Id { get; set; }

        [JsonPropertyName("gateway")]
        public string Gateway { get; set; }

        [JsonPropertyName("transactionDate")]
        public string TransactionDate { get; set; }

        [JsonPropertyName("accountNumber")]
        public string AccountNumber { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; } // "TT12..."

        [JsonPropertyName("transferType")]
        public string TransferType { get; set; }

        [JsonPropertyName("transferAmount")]
        public decimal TransferAmount { get; set; } // Sepay gửi số (10000)

        [JsonPropertyName("referenceCode")]
        public string ReferenceCode { get; set; } 

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
