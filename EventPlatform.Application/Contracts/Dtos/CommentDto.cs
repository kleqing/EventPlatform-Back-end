namespace EventPlatform.Application.Contracts.Dtos;

public class CommentDto
{
    public string UserName { get; set; }

    public byte Rating { get; set; }

    public string? Comment { get; set; }
    
    public DateTime? SubmittedAt { get; set; }
    public string? AvatarUrl { get; set; }
}