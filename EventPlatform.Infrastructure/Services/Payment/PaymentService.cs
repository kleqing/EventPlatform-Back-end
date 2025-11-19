using EventPlatform.Application.Contracts.Dtos;
using EventPlatform.Application.Services.Interfaces.Payment;
using EventPlatform.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EventPlatform.Infrastructure.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ISepayQrService _sepayService;
        private readonly string _webhookSecret;
        private readonly string _accNum;
        private readonly string _bankName;
        private readonly string _accName;

        public PaymentService(ApplicationDbContext context, ISepayQrService sepayService, IConfiguration configuration)
        {
            _context = context;
            _sepayService = sepayService;
            _webhookSecret = configuration["SEPAY_WEBHOOK_SECRET"];
            _accNum = configuration["SEPAY_ACCOUNT_NUMBER"];
            _bankName = configuration["SEPAY_BANK_NAME"];
            _accName = configuration["SEPAY_ACCOUNT_NAME"] ?? "EVENT PLATFORM";
        }

        public async Task<SepayQrResponse> InitiateSepayPaymentAsync(Guid registrationId)
        {
            // Tìm giao dịch đang chờ thanh toán
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.RegistrationId == registrationId && t.PaymentStatus == "Pending");

            if (transaction == null)
            {
                throw new Exception("Không tìm thấy giao dịch đang chờ cho đăng ký này.");
            }

            // Tạo nội dung chuyển khoản duy nhất dùng TransactionId (int) 
            var description = $"TT-{transaction.TransactionId}";

            var qrCodeUrl = _sepayService.GeneratePaymentQrCodeUrl(transaction.Amount, description);

            return new SepayQrResponse
            {
                QrCodeUrl = qrCodeUrl,
                TransactionDescription = description, 
                Amount = transaction.Amount,
                AccountNumber = _accNum,
                BankName = _bankName,
                AccountName = _accName
            };
        }

        public async Task HandleSepayWebhookAsync(SepayWebhookPayload payload, string apiKeyHeader)
        {
            // Bước 1: Xác thực Webhook 
            if (string.IsNullOrEmpty(apiKeyHeader) || apiKeyHeader != _webhookSecret)
            {
                throw new AuthenticationException("Invalid webhook API key.");
            }

            // Bước 2: Kiểm tra Idempotency (Chống lặp)
            // Chúng ta dùng 'reference_number' và lưu vào 'GatewayTransactionId'
            var existing = await _context.Transactions
                .AnyAsync(t => t.GatewayTransactionId == payload.ReferenceNumber);

            if (existing)
            {
                return;
            }

            // Bước 3: Phân tích nội dung lấy mã giao dịch 
            var transactionId = ParseTransactionIdFromContent(payload.TransactionContent);
            if (transactionId == 0)
            {
                throw new InvalidOperationException("Không thể phân tích Transaction ID từ nội dung.");
            }

            // Bước 4: Xác minh giao dịch 
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);

            if (transaction == null || transaction.PaymentStatus != "Pending")
            {
                throw new KeyNotFoundException("Giao dịch không tìm thấy hoặc đã được xử lý.");
            }

            if (transaction.PaymentStatus != "Pending")
            {
                throw new InvalidOperationException("Giao dịch không ở trạng thái Chờ (Pending).");
            }

            if (!decimal.TryParse(payload.AmountIn, out var paidAmount) || paidAmount != transaction.Amount)
            {
                // TODO: Ghi log lại trường hợp sai số tiền
                throw new InvalidOperationException("Số tiền không khớp.");
            }

            // Bước 5: Cập nhật CSDL (Atomicity)
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                transaction.PaymentStatus = "Success";
                transaction.GatewayTransactionId = payload.ReferenceNumber; // Quan trọng cho idempotency
                transaction.TransactionDate = DateTime.Parse(payload.TransactionDate);

                _context.Transactions.Update(transaction);

                // TODO: Cập nhật trạng thái 'Registration' sang "Đã thanh toán"

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                // TODO: Gửi thông báo SignalR cho client
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                throw new Exception("Lỗi khi cập nhật cơ sở dữ liệu.", ex); 
            }
        }

        private int ParseTransactionIdFromContent(string content)
        {
            // Nội dung của chúng ta là "TT-12345"
            var match = Regex.Match(content, @"TT-(\d+)");
            if (match.Success && int.TryParse(match.Groups[1].Value, out var transactionId))
            {
                return transactionId;
            }
            return 0; // Không tìm thấy
        }
    }
}
