namespace EventPlatform.Application.Contracts.Dtos;

public class EventDto
{
    public int EventId { get; set; }
    public string Title { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string EventType { get; set; } = null!;
    public string? Location { get; set; }
    public string? OnlineUrl { get; set; }
    public string? CardImageUrl { get; set; }
    public string Description { get; set; } = null!;

}