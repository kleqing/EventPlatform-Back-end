using System;

namespace EventPlatform.Application.Contracts.Dtos
{
    public class UserRegistrationDto
    {
        public Guid RegistrationId { get; set; }
        public Guid UserId { get; set; }
        public int EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? OnlineUrl { get; set; }
        public string EventStatus { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public int TicketTypeId { get; set; }
        public string TicketTypeName { get; set; } = string.Empty;
        public decimal TicketPrice { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string UniqueToken { get; set; } = string.Empty;
        public DateTime? CheckInTime { get; set; }
    }
}
