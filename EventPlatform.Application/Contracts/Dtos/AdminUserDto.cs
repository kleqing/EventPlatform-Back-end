using System;

namespace EventPlatform.Application.Contracts.Dtos
{
    public class AdminUserDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string AccountStatus { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public int TotalRegistrations { get; set; }
    }
}
