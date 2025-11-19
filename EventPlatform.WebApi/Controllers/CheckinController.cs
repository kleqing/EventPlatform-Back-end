using EventPlatform.Application.Contracts.Requests;
using EventPlatform.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventPlatform.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckinController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CheckinController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // 1. LOGIC CHO EVENT ONLINE (Người dùng tự click link)
        // ============================================================
        // Link này sẽ được gửi trong email: https://domain.com/api/Checkin/join-online?token=...
        [HttpGet("join-online")]
        public async Task<IActionResult> JoinOnlineEvent(string token)
        {
            if (string.IsNullOrEmpty(token)) return BadRequest("Token không hợp lệ.");

            // 1. Tìm vé dựa trên Token
            var registration = await _context.Registrations
                .Include(r => r.TicketType)
                    .ThenInclude(tt => tt.Event)
                .Include(r => r.Transactions) 
                .FirstOrDefaultAsync(r => r.UniqueToken == token);

            if (registration == null)
                return NotFound("Vé không tồn tại.");

            // Kiểm tra thanh toán (Transaction thành công mới được vào)
            var isPaid = registration.Transactions.Any(t => t.PaymentStatus == "Success");
            if (!isPaid)
                return Content("Lỗi: Vé chưa được thanh toán thành công.");

            // Kiểm tra đúng là event Online không
            if (registration.TicketType.Event.EventType != "Online")
                return Content("Lỗi: Đây là vé cho sự kiện Offline.");

            // Cập nhật thời gian Check-in (Nếu là lần đầu vào)
            if (registration.CheckInTime == null)
            {
                registration.CheckInTime = DateTime.UtcNow;
                _context.Registrations.Update(registration);
                await _context.SaveChangesAsync();
            }

            // CHUYỂN HƯỚNG sang link họp (Zoom/Meet/Teams)
            // Người dùng sẽ không biết link gốc cho đến khi được verify xong
            string meetingUrl = registration.TicketType.Event.OnlineUrl;
            return Redirect(meetingUrl);
        }

        // ============================================================
        // 2. LOGIC CHO EVENT OFFLINE (Ban tổ chức quét mã QR)
        // ============================================================
        [HttpPost("verify-offline")]
        // [Authorize(Roles = "Admin,Speaker")] // Bỏ comment để chỉ cho phép BTC quét
        public async Task<IActionResult> VerifyOfflineTicket([FromBody] CheckinRequest request)
        {
            var registration = await _context.Registrations
                .Include(r => r.User)
                .Include(r => r.TicketType)
                    .ThenInclude(tt => tt.Event)
                .Include(r => r.Transactions)
                .FirstOrDefaultAsync(r => r.UniqueToken == request.Token);

            if (registration == null)
                return Ok(new { isValid = false, message = "❌ Mã vé không tồn tại!" });

            var isPaid = registration.Transactions.Any(t => t.PaymentStatus == "Success");
            if (!isPaid)
                return Ok(new { isValid = false, message = "⚠️ Vé chưa thanh toán!" });

            // 4. Kiểm tra xem đã check-in trước đó chưa? (Chống dùng lại vé)
            if (registration.CheckInTime != null)
            {
                return Ok(new
                {
                    isValid = false,
                    message = $"⚠️ CẢNH BÁO: Vé đã được sử dụng lúc {registration.CheckInTime.Value.ToLocalTime():HH:mm dd/MM}!",
                    user = registration.User.FullName
                });
            }

            // 5. Hợp lệ -> Update DB
            registration.CheckInTime = DateTime.UtcNow;
            _context.Registrations.Update(registration);
            await _context.SaveChangesAsync();

            // 6. Trả về thông tin khách để BTC đối chiếu
            return Ok(new
            {
                isValid = true,
                message = "✅ Check-in THÀNH CÔNG!",
                customerName = registration.User.FullName,
                ticketType = registration.TicketType.Name,
                eventName = registration.TicketType.Event.Title
            });
        }
    }
}