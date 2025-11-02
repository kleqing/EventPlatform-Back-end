using EventPlatform.Application.Contracts.Requests;
using EventPlatform.Domain.Entities;

namespace EventPlatform.Application.Contracts.Interfaces;

public interface IForumRepository
{
    Task<List<ForumCategory>> ListForumCategoriesAsync();
    Task<string> DisplayForumTitleAsync(int postId);
    Task<List<ForumPost>> ListForumPostsAsync(int? categoryId = null);
    Task<ForumPost?> GetForumPostDetailAsync(int postId);
    Task<ForumPost> CreateForumPostAsync(CreateForumPostRequest request);
    Task<int> CountAllForumThreadsAsync();
    Task<int> CountAllMembersAsync();
    Task<int> CountAllActiveForumThreadAsync();
    Task<ForumComment> CreateForumCommentAsync(CreateForumCommentRequest request);
    Task<int> CountCommentOfEachListPostAsync(int postId);
    Task<(bool isLiked, int likeCount)> ToggleLikeForumCommentAsync(int commentId, Guid userId);
    Task<(bool isLiked, int likeCount)> ToggleLikePostForumAsync(int postId, Guid userId);
    Task<int> CountLikeOfPostAsync(int postId);
    Task<int> CountLikeOfCommentAsync(int commentId);
    Task<bool> IsPostLikedByUserAsync(int postId, Guid userId);
    Task<bool> IsCommentLikedByUserAsync(int commentId, Guid userId);
}
