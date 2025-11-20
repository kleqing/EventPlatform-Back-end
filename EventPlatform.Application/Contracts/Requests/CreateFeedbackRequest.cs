namespace EventPlatform.Application.Contracts.Requests;

public class CreateFeedbackRequest
{
    public int EventId { get; set; }

    public string UserEmail { get; set; }

    public byte Rating { get; set; }

    public string? Comment { get; set; }
    
    public DateTime? SubmittedAt { get; set; }
}