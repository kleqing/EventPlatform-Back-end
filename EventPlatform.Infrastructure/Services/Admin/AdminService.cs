using System;
using System.Linq;
using System.Threading.Tasks;
using EventPlatform.Application.Contracts.Dtos;
using EventPlatform.Application.Services.Interfaces.Admin;
using EventPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventPlatform.Infrastructure.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardSummaryDto> GetDashboardSummaryAsync()
        {
            var totalEvents = await _context.Events.CountAsync();
            var pendingEvents = await _context.Events.CountAsync(e => e.EventStatus == "Pending");
            var totalUsers = await _context.Users.CountAsync();
            var totalTransactions = await _context.Transactions.CountAsync();
            var totalRevenue = await _context.Transactions
                .Where(t => t.PaymentStatus == "Success" || t.PaymentStatus == "Completed")
                .SumAsync(t => (decimal?)t.Amount) ?? 0m;

            return new AdminDashboardSummaryDto
            {
                TotalEvents = totalEvents,
                PendingEvents = pendingEvents,
                TotalUsers = totalUsers,
                TotalTransactions = totalTransactions,
                TotalRevenue = totalRevenue
            };
        }

        public async Task<PaginatedResult<AdminEventDto>> GetEventsAsync(string? status, string? keyword, int pageNumber, int pageSize)
        {
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.Events
                .AsNoTracking()
                .Include(e => e.CreatedByUser)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(e => e.EventStatus == status);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(e => e.Title.Contains(keyword) || e.Description.Contains(keyword));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(e => e.CreatedAt ?? e.StartTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new AdminEventDto
                {
                    EventId = e.EventId,
                    Title = e.Title,
                    EventType = e.EventType,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    EventStatus = e.EventStatus,
                    Location = e.Location,
                    OnlineUrl = e.OnlineUrl,
                    CreatedAt = e.CreatedAt,
                    SubmittedByName = e.CreatedByUser.FullName,
                    SubmittedByEmail = e.CreatedByUser.Email
                })
                .ToListAsync();

            return new PaginatedResult<AdminEventDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<bool> UpdateEventStatusAsync(int eventId, string newStatus)
        {
            var eventEntity = await _context.Events.FindAsync(eventId);
            if (eventEntity == null)
            {
                return false;
            }

            if (!string.Equals(eventEntity.EventStatus, newStatus, StringComparison.OrdinalIgnoreCase))
            {
                eventEntity.EventStatus = newStatus;
                eventEntity.UpdatedAt = DateTime.UtcNow;
                _context.Events.Update(eventEntity);
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<PaginatedResult<AdminTransactionDto>> GetTransactionsAsync(string? status, int pageNumber, int pageSize)
        {
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.Transactions
                .AsNoTracking()
                .Include(t => t.Registration)
                    .ThenInclude(r => r.User)
                .Include(t => t.Registration)
                    .ThenInclude(r => r.TicketType)
                        .ThenInclude(tt => tt.Event)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(t => t.PaymentStatus == status);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(t => t.TransactionDate ?? DateTime.MinValue)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new AdminTransactionDto
                {
                    TransactionId = t.TransactionId,
                    Amount = t.Amount,
                    PaymentStatus = t.PaymentStatus,
                    PaymentGateway = t.PaymentGateway,
                    GatewayTransactionId = t.GatewayTransactionId,
                    TransactionDate = t.TransactionDate,
                    BuyerName = t.Registration.User.FullName,
                    BuyerEmail = t.Registration.User.Email,
                    EventTitle = t.Registration.TicketType.Event.Title
                })
                .ToListAsync();

            return new PaginatedResult<AdminTransactionDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PaginatedResult<AdminUserDto>> GetUsersAsync(string? role, string? accountStatus, int pageNumber, int pageSize)
        {
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _context.Users.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(role))
            {
                query = query.Where(u => u.Role == role);
            }

            if (!string.IsNullOrWhiteSpace(accountStatus))
            {
                query = query.Where(u => u.AccountStatus == accountStatus);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new AdminUserDto
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    AccountStatus = u.AccountStatus,
                    CreatedAt = u.CreatedAt,
                    TotalRegistrations = u.Registrations.Count
                })
                .ToListAsync();

            return new PaginatedResult<AdminUserDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<bool> UpdateUserStatusAsync(Guid userId, string newStatus)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            if (!string.Equals(user.AccountStatus, newStatus, StringComparison.OrdinalIgnoreCase))
            {
                user.AccountStatus = newStatus;
                user.UpdatedAt = DateTime.UtcNow;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }

            return true;
        }
    }
}
