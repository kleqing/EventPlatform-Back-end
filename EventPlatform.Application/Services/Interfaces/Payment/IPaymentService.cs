using EventPlatform.Application.Contracts.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPlatform.Application.Services.Interfaces.Payment
{
    public interface IPaymentService
    {
        // Tạo thanh toán, trả về link QR
        Task<SepayQrResponse> InitiateSepayPaymentAsync(Guid registrationId);

        // Xử lý webhook từ Sepay
        Task HandleSepayWebhookAsync(SepayWebhookPayload payload, string apiKeyHeader);
        Task<string?> GetPaymentStatusAsync(Guid registrationId);
    }
}
