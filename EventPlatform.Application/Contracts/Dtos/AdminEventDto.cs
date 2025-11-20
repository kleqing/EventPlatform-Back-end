using System;

namespace EventPlatform.Application.Contracts.Dtos
{
    public class AdminEventDto
    {
        public int EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string EventStatus { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? OnlineUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string SubmittedByName { get; set; } = string.Empty;
        public string SubmittedByEmail { get; set; } = string.Empty;
    }
}
