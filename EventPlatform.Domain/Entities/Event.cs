using System;
using System.Collections.Generic;

namespace EventPlatform.Domain.Entities;

public partial class Event
{
    public Guid EventId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? ThumbnailUrl { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string EventType { get; set; } = null!;

    public string? Location { get; set; }

    public string? OnlineUrl { get; set; }

    public string EventStatus { get; set; } = null!;

    public Guid CreatedByUserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CoverImageUrl { get; set; }

    public string? CardImageUrl { get; set; }

    public string? OrganizerName { get; set; }

    public string? OrganizerInfo { get; set; }

    public string? OrganizerLogoUrl { get; set; }

    public string? VenueName { get; set; }

    public string? AddressStreet { get; set; }

    public string? AddressWard { get; set; }

    public string? AddressDistrict { get; set; }

    public string? AddressCity { get; set; }

    public Guid? CategoryId { get; set; }

    public virtual EventCategory? Category { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<TicketType> TicketTypes { get; set; } = new List<TicketType>();

    public virtual ICollection<SpeakerProfile> Speakers { get; set; } = new List<SpeakerProfile>();

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
