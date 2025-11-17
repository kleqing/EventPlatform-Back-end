using System;

namespace EventPlatform.Application.Contracts.Dtos;

public class AppliedEventDto
{
    public int EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Location { get; set; }
    public string? OnlineUrl { get; set; }
    public string EventStatus { get; set; } = string.Empty;
}
