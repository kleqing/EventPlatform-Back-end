using System;

namespace EventPlatform.Application.Contracts.Dtos
{
    public class AdminDashboardSummaryDto
    {
        public int TotalEvents { get; set; }
        public int PendingEvents { get; set; }
        public int TotalUsers { get; set; }
        public int TotalTransactions { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
