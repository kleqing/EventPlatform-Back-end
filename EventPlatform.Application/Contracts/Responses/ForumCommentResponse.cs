namespace EventPlatform.Application.Contracts.Responses;

public class ForumCommentResponse
{
    public int CommentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int LikeCount { get; set; }
    public bool IsLikedByCurrentUser { get; set; }
}