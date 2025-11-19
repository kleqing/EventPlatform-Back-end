namespace EventPlatform.Application.Contracts.Requests;

public class CreateForumCommentRequest
{
    public int PostId { get; set; }
    public Guid UserId { get; set; }
    public int? ParentCommentId { get; set; }
    public string Content { get; set; } = null!;
}