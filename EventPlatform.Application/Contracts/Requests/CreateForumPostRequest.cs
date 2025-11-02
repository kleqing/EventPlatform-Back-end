namespace EventPlatform.Application.Contracts.Requests;

public class CreateForumPostRequest
{
    public Guid UserId { get; set; }
    public int CategoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}