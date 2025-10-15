using System;
using System.Collections.Generic;

namespace EventPlatform.Domain.Entities;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int EventId { get; set; }

    public int UserId { get; set; }

    public byte Rating { get; set; }

    public string? Comment { get; set; }

    public double? SentimentScore { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
