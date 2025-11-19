using EventPlatform.Application.Contracts.Dtos;
using EventPlatform.Application.Services.Interfaces.Email;
using EventPlatform.Application.Services.Interfaces.Payment;
using EventPlatform.Domain.Entities;
using EventPlatform.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
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
        private readonly IEmailSender _emailSender;

        public PaymentService(ApplicationDbContext context, ISepayQrService sepayService, IConfiguration configuration, IEmailSender emailSender)
        {
            _context = context;
            _sepayService = sepayService;
            _webhookSecret = configuration["SEPAY_WEBHOOK_SECRET"];
            _accNum = configuration["SEPAY_ACCOUNT_NUMBER"];
            _bankName = configuration["SEPAY_BANK_NAME"];
            _accName = configuration["SEPAY_ACCOUNT_NAME"] ?? "EVENT PLATFORM";
            _emailSender = emailSender;
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
            // 1. Xác thực
            if (string.IsNullOrEmpty(apiKeyHeader) || apiKeyHeader != _webhookSecret)
            {
                throw new AuthenticationException("Invalid webhook API key.");
            }

            // 2. Check Idempotency (Chống lặp)
            // Lưu ý: Kiểm tra null cho ReferenceCode
            if (!string.IsNullOrEmpty(payload.ReferenceCode))
            {
                var existing = await _context.Transactions
                    .AnyAsync(t => t.GatewayTransactionId == payload.ReferenceCode);
                if (existing) return;
            }

            // 3. Parse ID
            // Log nội dung ra nếu cần thiết
            var transactionId = ParseTransactionIdFromContent(payload.Content);
            if (transactionId == 0)
            {
                throw new InvalidOperationException($"Không tìm thấy ID giao dịch trong nội dung: '{payload.Content}'");
            }

            // 4. Tìm giao dịch trong DB
            var transaction = await _context.Transactions.Include(t => t.Registration)
                    .ThenInclude(r => r.User)
                    .Include(t => t.Registration)
                    .ThenInclude(r => r.TicketType)
                    .ThenInclude(tt => tt.Event) 
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);

            if (transaction == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy TransactionID: {transactionId} trong DB.");
            }

            if (transaction.PaymentStatus == "Success") return;

            // 5. Validate số tiền (Cho phép sai số nhỏ nếu cần, nhưng ở đây so sánh chính xác)
            if (payload.TransferAmount < transaction.Amount)
            {
                throw new InvalidOperationException($"Tiền thiếu. Cần: {transaction.Amount}, Nhận: {payload.TransferAmount}");
            }

            // 6. Update DB
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                transaction.PaymentStatus = "Success";
                transaction.GatewayTransactionId = payload.ReferenceCode; 

                // Xử lý ngày tháng an toàn (Tránh lỗi nếu null)
                if (!string.IsNullOrEmpty(payload.TransactionDate) &&
                    DateTime.TryParseExact(payload.TransactionDate, "yyyy-MM-dd HH:mm:ss",
                    System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var transDate))
                {
                    transaction.TransactionDate = transDate;
                }
                else
                {
                    transaction.TransactionDate = DateTime.UtcNow; // Fallback nếu lỗi format hoặc null
                }

                var registration = transaction.Registration;
                _context.Transactions.Update(transaction);

                // TODO: Cập nhật bảng Registration

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();
                try
                {
                    await SendTicketEmailAsync(transaction);
                }
                catch (Exception emailEx)
                {
                    Console.WriteLine($"Lỗi gửi mail: {emailEx.Message}");
                }
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                // Ném lỗi kèm InnerException để thấy rõ nguyên nhân (ví dụ: String truncated)
                throw new Exception($"Lỗi cập nhật DB: {ex.Message} - {ex.InnerException?.Message}", ex);
            }
        }

        private int ParseTransactionIdFromContent(string content)
        {
            // Nội dung của chúng ta là "TT-12345"
            var match = Regex.Match(content, @"(?i)TT\W*(\d+)");
            if (match.Success && int.TryParse(match.Groups[1].Value, out var transactionId))
            {
                return transactionId;
            }
            return 0; // Không tìm thấy
        }

        public async Task<string?> GetPaymentStatusAsync(Guid registrationId)
        {
            var transaction = await _context.Transactions
                .Where(t => t.RegistrationId == registrationId)
                .OrderByDescending(t => t.TransactionDate) // Lấy giao dịch mới nhất
                .FirstOrDefaultAsync();

            return transaction?.PaymentStatus;
        }


        // --- HÀM PRIVATE HỖ TRỢ GỬI MAIL ---
        private async Task SendTicketEmailAsync(Transaction transaction)
        {
            var registration = transaction.Registration;
            var user = registration.User;
            var evt = registration.TicketType.Event;
            var ticketType = registration.TicketType;

            string subject = $"[Vé Điện Tử] {evt.Title} - Thanh toán thành công";
            string body = "";

            // Format tiền tệ
            var priceStr = transaction.Amount.ToString("N0", new CultureInfo("vi-VN"));
            var dateStr = evt.StartTime.ToString("dd/MM/yyyy HH:mm");

            // Nội dung chung
            string commonInfo = $@"
                <h2>Thanh toán thành công!</h2>
                <p>Xin chào <strong>{user.FullName}</strong>,</p>
                <p>Cảm ơn bạn đã đặt vé. Dưới đây là thông tin vé của bạn:</p>
                <hr/>
                <ul>
                    <li><strong>Sự kiện:</strong> {evt.Title}</li>
                    <li><strong>Thời gian:</strong> {dateStr}</li>
                    <li><strong>Loại vé:</strong> {ticketType.Name}</li>
                    <li><strong>Giá vé:</strong> {priceStr} VNĐ</li>
                    <li><strong>Mã đơn hàng:</strong> #{transaction.TransactionId}</li>
                </ul>";

            // Xử lý logic Online vs Offline
            if (evt.EventType == "Online")
            {
                string joinLink = evt.OnlineUrl ?? "#";
                // Tạo link tham gia kèm token để verify (nếu hệ thống hỗ trợ)
                // var secureLink = $"{joinLink}?token={registration.UniqueToken}"; 

                body = commonInfo + $@"
                <div style='background-color: #e3f2fd; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                    <h3>ℹ️ Hướng dẫn tham gia Online</h3>
                    <p>Sự kiện sẽ diễn ra trực tuyến. Vui lòng truy cập vào đường dẫn dưới đây đúng giờ:</p>
                    <p style='text-align: center;'>
                        <a href='{joinLink}' style='background-color: #1976d2; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; font-weight: bold;'>
                            THAM GIA NGAY
                        </a>
                    </p>
                    <p><em>Mã vé của bạn: <strong>{registration.UniqueToken}</strong></em></p>
                </div>";
            }
            else // Offline
            {
                // 1. Tạo object chứa thông tin hiển thị trong QR
                var qrPayload = new
                {
                    Token = registration.UniqueToken, // Cái này quan trọng nhất để verify
                    Event = evt.Title.Length > 20 ? evt.Title.Substring(0, 17) + "..." : evt.Title, 
                    User = user.FullName,
                    Type = ticketType.Name
                };

                // 2. Chuyển sang JSON string: {"Token":"...","Event":"...","User":"..."}
                string jsonString = JsonSerializer.Serialize(qrPayload);

                // 3. Mã hóa URL để gửi qua API (vì JSON chứa ký tự đặc biệt như { } " :)
                string encodedData = Uri.EscapeDataString(jsonString);

                // 4. Gọi API tạo QR
                // Lưu ý: QR chứa nhiều thông tin thì điểm ảnh sẽ nhỏ hơn, nên tăng size lên 300x300
                string qrUrl = $"https://api.qrserver.com/v1/create-qr-code/?size=300x300&data={encodedData}";

                string location = evt.Location ?? evt.VenueName;

                body = commonInfo + $@"
    <div style='background-color: #f1f8e9; padding: 15px; border-radius: 5px; margin: 20px 0;'>
        <h3>📍 Vé Check-in Sự Kiện</h3>
        <p><strong>Địa điểm:</strong> {location}</p>
        <p>Vui lòng đưa mã QR này cho nhân viên soát vé:</p>
        <div style='text-align: center; margin: 20px;'>
            <img src='{qrUrl}' alt='Ticket QR Code' style='border: 2px solid #333; padding: 5px; border-radius: 5px; width: 200px; height: 200px;'/>
            <p style='margin-top: 10px; font-family: monospace; color: #555;'>{registration.UniqueToken}</p>
        </div>
    </div>";
            }

            // Footer
            body += "<p>Cảm ơn bạn đã sử dụng EventPlatform!</p>";

            await _emailSender.SendEmailAsync(user.Email, subject, body);
        }
    }
}
