using EventPlatform.Application.Contracts.Requests;
using EventPlatform.Application.Contracts.Responses;
using EventPlatform.Application.Services.Interfaces;
using EventPlatform.Application.Services.Interfaces.Event;
using EventPlatform.Domain.Entities;
using EventPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPlatform.Infrastructure.Services.Event
{
    public class RegistrationService : IRegistrationService
    {
        private readonly ApplicationDbContext _context; 
        private readonly ICurrentUserService _currentUserService;

        public RegistrationService(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<BookingResponse> CreateBookingAsync(CreateBookingRequest request)
        {
            // 1. Lấy User ID từ Token (Bảo mật)
            var userId = _currentUserService.GetCurrentUserId();
            if (userId == null) throw new UnauthorizedAccessException("Bạn cần đăng nhập để đặt vé.");

            // 2. Bắt đầu Transaction Database (để đảm bảo tính toàn vẹn: tạo vé + trừ kho + tạo transaction phải thành công cùng nhau)
            using var dbTransaction = await _context.Database.BeginTransactionAsync();

            try
            {
                decimal totalAmount = 0;
                int totalTickets = 0;
                var newRegistrations = new List<Registration>();

                foreach (var item in request.Tickets)
                {
                    if (item.Quantity <= 0) continue;

                    // Kiểm tra TicketType và Tồn kho (Lock row nếu cần thiết, ở đây dùng check đơn giản)
                    var ticketType = await _context.TicketTypes.FindAsync(item.TicketTypeId);

                    if (ticketType == null)
                        throw new KeyNotFoundException($"Loại vé ID {item.TicketTypeId} không tồn tại.");

                    if (ticketType.EventId != request.EventId)
                        throw new InvalidOperationException("Loại vé không thuộc về sự kiện này.");

                    if (ticketType.AvailableQuantity < item.Quantity)
                        throw new InvalidOperationException($"Vé '{ticketType.Name}' chỉ còn lại {ticketType.AvailableQuantity} vé.");

                    // Trừ tồn kho
                    ticketType.AvailableQuantity -= item.Quantity;
                    _context.TicketTypes.Update(ticketType);

                    // Tính tiền
                    totalAmount += ticketType.Price * item.Quantity;
                    totalTickets += item.Quantity;

                    // Tạo các bản ghi Registration (1 vé = 1 bản ghi để có mã QR riêng sau này)
                    for (int i = 0; i < item.Quantity; i++)
                    {
                        newRegistrations.Add(new Registration
                        {
                            RegistrationId = Guid.NewGuid(),
                            UserId = userId.Value,
                            TicketTypeId = ticketType.TicketTypeId,
                            RegistrationDate = DateTime.UtcNow,
                            UniqueToken = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper(), 
                            CheckInTime = null
                        });
                    }
                }

                if (!newRegistrations.Any()) throw new InvalidOperationException("Không có vé nào được chọn.");

                // 4. Lưu Registrations
                await _context.Registrations.AddRangeAsync(newRegistrations);

                // 5. Tạo Transaction thanh toán (Pending)
                // Lưu ý: Chúng ta gắn Transaction vào Registration đầu tiên để làm đại diện (Main ID)
                var mainRegistrationId = newRegistrations.First().RegistrationId;

                var transaction = new Transaction
                {
                    RegistrationId = mainRegistrationId, // Link vào vé đầu tiên
                    Amount = totalAmount,
                    PaymentStatus = "Pending",
                    PaymentGateway = "Sepay",
                    TransactionDate = DateTime.UtcNow
                    // GatewayTransactionId sẽ được điền khi Webhook gọi về
                };

                await _context.Transactions.AddAsync(transaction);

                // 6. Commit tất cả xuống DB
                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return new BookingResponse
                {
                    MainRegistrationId = mainRegistrationId,
                    TotalAmount = totalAmount,
                    TotalTickets = totalTickets,
                    PaymentStatus = "Pending"
                };
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw; // Ném lỗi ra để Controller bắt
            }
        }

        public async Task CancelBookingAsync(CancelBookingRequest request)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.RegistrationId == request.RegistrationId);

            // Chỉ hủy nếu giao dịch còn đang Pending (chưa thanh toán thành công)
            if (transaction != null && transaction.PaymentStatus == "Pending")
            {
                using var dbTransaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 2. Cập nhật trạng thái Transaction thành Cancelled
                    transaction.PaymentStatus = "Cancelled";
                    _context.Transactions.Update(transaction);

                    // 3. HOÀN VÉ VÀO KHO (Restore Inventory)
                    foreach (var item in request.Tickets)
                    {
                        var ticketType = await _context.TicketTypes.FindAsync(item.TicketTypeId);
                        if (ticketType != null)
                        {
                            ticketType.AvailableQuantity += item.Quantity; // Cộng lại số lượng
                            _context.TicketTypes.Update(ticketType);
                        }
                    }

                    // 4. Xóa hoặc đánh dấu hủy các bản ghi Registration (Tùy logic, giữ lại nhưng đánh dấu transaction hủy)
                    // Nếu muốn xóa sạch: 
                    // var regs = _context.Registrations.Where(r => r.UserId == ... && r.TicketTypeId == ...);
                    // _context.Registrations.RemoveRange(regs);

                    await _context.SaveChangesAsync();
                    await dbTransaction.CommitAsync();
                }
                catch
                {
                    await dbTransaction.RollbackAsync();
                    throw;
                }
            }
        }
    }
}
