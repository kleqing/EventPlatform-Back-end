using System;
using System.Threading.Tasks;
using EventPlatform.Application.Contracts.Dtos;

namespace EventPlatform.Application.Services.Interfaces.Admin
{
    public interface IAdminService
    {
        Task<AdminDashboardSummaryDto> GetDashboardSummaryAsync();
        Task<PaginatedResult<AdminEventDto>> GetEventsAsync(string? status, string? keyword, int pageNumber, int pageSize);
        Task<bool> UpdateEventStatusAsync(int eventId, string newStatus);
        Task<PaginatedResult<AdminTransactionDto>> GetTransactionsAsync(string? status, int pageNumber, int pageSize);
        Task<PaginatedResult<AdminUserDto>> GetUsersAsync(string? role, string? accountStatus, int pageNumber, int pageSize);
        Task<bool> UpdateUserStatusAsync(Guid userId, string newStatus);
    }
}
