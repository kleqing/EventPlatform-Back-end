using System;
using System.Collections.Generic;

namespace EventPlatform.Domain.Entities;

public partial class SpeakerProfile
{
    public int SpeakerId { get; set; }

    public int UserId { get; set; }

    public string? Bio { get; set; }

    public string? Topics { get; set; }

    public string? Company { get; set; }

    public string? JobTitle { get; set; }

    public string ApprovalStatus { get; set; } = null!;

    public string? WebsiteUrl { get; set; }

    public string? LinkedInUrl { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
