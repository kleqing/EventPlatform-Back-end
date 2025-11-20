using System;
using System.Threading.Tasks;
using EventPlatform.Application.Contracts.Dtos;
using EventPlatform.Application.Contracts.Requests.Admin;
using EventPlatform.Application.Services.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventPlatform.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("dashboard/summary")]
        public async Task<ActionResult<AdminDashboardSummaryDto>> GetDashboardSummary()
        {
            var summary = await _adminService.GetDashboardSummaryAsync();
            return Ok(summary);
        }

        [HttpGet("events")]
        public async Task<ActionResult<PaginatedResult<AdminEventDto>>> GetEvents([FromQuery] string? status, [FromQuery] string? keyword, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var events = await _adminService.GetEventsAsync(status, keyword, pageNumber, pageSize);
            return Ok(events);
        }

        [HttpPost("events/{eventId:int}/approve")]
        public async Task<IActionResult> ApproveEvent(int eventId)
        {
            var updated = await _adminService.UpdateEventStatusAsync(eventId, "Approved");
            if (!updated)
            {
                return NotFound(new { message = "Event not found." });
            }

            return NoContent();
        }

        [HttpPost("events/{eventId:int}/reject")]
        public async Task<IActionResult> RejectEvent(int eventId, [FromBody] RejectEventRequest request)
        {
            var updated = await _adminService.UpdateEventStatusAsync(eventId, "Rejected");
            if (!updated)
            {
                return NotFound(new { message = "Event not found." });
            }

            return NoContent();
        }

        [HttpGet("transactions")]
        public async Task<ActionResult<PaginatedResult<AdminTransactionDto>>> GetTransactions([FromQuery] string? status, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var transactions = await _adminService.GetTransactionsAsync(status, pageNumber, pageSize);
            return Ok(transactions);
        }

        [HttpGet("users")]
        public async Task<ActionResult<PaginatedResult<AdminUserDto>>> GetUsers([FromQuery] string? role, [FromQuery] string? accountStatus, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var users = await _adminService.GetUsersAsync(role, accountStatus, pageNumber, pageSize);
            return Ok(users);
        }

        [HttpPost("users/{userId:guid}/ban")]
        public async Task<IActionResult> BanUser(Guid userId)
        {
            var updated = await _adminService.UpdateUserStatusAsync(userId, "Banned");
            if (!updated)
            {
                return NotFound(new { message = "User not found." });
            }

            return NoContent();
        }

        [HttpPost("users/{userId:guid}/unban")]
        public async Task<IActionResult> UnbanUser(Guid userId)
        {
            var updated = await _adminService.UpdateUserStatusAsync(userId, "Active");
            if (!updated)
            {
                return NotFound(new { message = "User not found." });
            }

            return NoContent();
        }
    }
}
