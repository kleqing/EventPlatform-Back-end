using EventPlatform.Application.Contracts.Requests;
using EventPlatform.Application.Interfaces;
using EventPlatform.Domain.Entities;
using EventPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventPlatform.Infrastructure.Repositories;

public class ForumRepository : IForumRepository
{
    private readonly ApplicationDbContext _context;

    public ForumRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    //* Index page
    public async Task<List<ForumCategory>> ListForumCategoriesAsync()
    {
        return await _context.ForumCategories.AsNoTracking().ToListAsync();
    }
    
    public async Task<string> DisplayForumTitleAsync(int postId)
    {
        return await _context.ForumPosts.Where(x => x.PostId == postId)
            .Select(x => x.Title)
            .FirstOrDefaultAsync() ?? string.Empty;
    }
    
    public async Task<List<ForumPost>> ListForumPostsAsync(int? categoryId = null)
    {
        IQueryable<ForumPost> query = _context.ForumPosts
            .AsNoTracking()
            .Select(x => new ForumPost
            {
                PostId = x.PostId,
                UserId = x.UserId,
                Title = x.Title,
                Content = x.Content,
                PostStatus = x.PostStatus,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,

                CategoryId = x.CategoryId,

                Category = new ForumCategory
                {
                    CategoryId = x.Category.CategoryId,
                    Name = x.Category.Name,
                    Description = x.Category.Description
                },

                User = new User
                {
                    UserId = x.User.UserId,
                    FullName = x.User.FullName,
                    Email = x.User.Email,
                    DateOfBirth = x.User.DateOfBirth,
                    AvatarUrl = x.User.AvatarUrl,
                    AccountStatus = x.User.AccountStatus,
                },
                CommentCount = _context.ForumComments.Count(c => c.PostId == x.PostId),
            });

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<ForumPost?> GetForumPostDetailAsync(int postId)
    {
        var forumDetail = await _context.ForumPosts
            .Where(p => p.PostId == postId)
            .Select(p => new ForumPost
            {
                PostId = p.PostId,
                UserId = p.UserId,
                CategoryId = p.CategoryId,
                Title = p.Title,
                Content = p.Content,
                PostStatus = p.PostStatus,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,

                Category = new ForumCategory
                {
                    CategoryId = p.Category.CategoryId,
                    Name = p.Category.Name,
                    Description = p.Category.Description
                },

                User = new User
                {
                    UserId = p.User.UserId,
                    FullName = p.User.FullName,
                    Email = p.User.Email
                },

                ForumComments = p.ForumComments.Select(c => new ForumComment
                {
                    CommentId = c.CommentId,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    ParentCommentId = c.ParentCommentId,

                    User = new User
                    {
                        UserId = c.User.UserId,
                        FullName = c.User.FullName
                    },

                    InverseParentComment = c.InverseParentComment
                        .Select(ic => new ForumComment
                        {
                            CommentId = ic.CommentId,
                            Content = ic.Content,
                            CreatedAt = ic.CreatedAt,
                            UserId = ic.UserId
                        })
                        .ToList()
                }).ToList()
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();

        return forumDetail;
    }
    
    public async Task<ForumPost> CreateForumPostAsync(CreateForumPostRequest request)
    {
        var post = new ForumPost
        {
            Title = request.Title,
            Content = request.Content,
            CategoryId = request.CategoryId,
            UserId = request.UserId,
            PostStatus = "Visible",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        _context.ForumPosts.Add(post);
        await _context.SaveChangesAsync();
        return post;
    }
    
    //* Status field
    public async Task<int> CountAllForumThreadsAsync()  
    {
        return await _context.ForumPosts.CountAsync();
    }
    
    public async Task<int> CountAllMembersAsync()
    {
        var postUserIds = _context.ForumPosts
            .Select(p => p.UserId);

        var commentUserIds = _context.ForumComments
            .Select(c => c.UserId);

        var uniqueMemberCount = await postUserIds
            .Union(commentUserIds)
            .Distinct()
            .CountAsync();

        return uniqueMemberCount;
    }

    public async Task<int> CountAllActiveForumThreadAsync()
    {
        var activeThreads = await _context.ForumPosts
            .Where(x => x.PostStatus == "Visible")
            .CountAsync();
        return activeThreads;
    }
    
    //* Comment post
    public async Task<ForumComment> CreateForumCommentAsync(CreateForumCommentRequest request)
    {
        var comment = new ForumComment
        {
            PostId = request.PostId,
            UserId = request.UserId,
            ParentCommentId = request.ParentCommentId > 0 ? request.ParentCommentId : null,
            Content = request.Content,
            CommentStatus = "Visible",
            CreatedAt = DateTime.UtcNow
        };
        
        _context.ForumComments.Add(comment);
        await _context.SaveChangesAsync();
        return comment;
    }
    
    public async Task<int> CountCommentOfEachListPostAsync(int postId)
    {
        return await _context.ForumComments
            .Where(c => c.PostId == postId)
            .CountAsync();
    }
    
    //* Like 
    
    public async Task<(bool isLiked, int likeCount)> ToggleLikeForumCommentAsync(int commentId, Guid userId)
    {
        var like = await _context.ForumLikes
            .FirstOrDefaultAsync(l => l.CommentId == commentId && l.UserId == userId);

        if (like != null)
        {
            _context.ForumLikes.Remove(like);
            await _context.SaveChangesAsync();
            var count = await _context.ForumLikes.CountAsync(l => l.CommentId == commentId);
            return (false, count);
        }

        _context.ForumLikes.Add(new ForumLike
        {
            CommentId = commentId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        var newCount = await _context.ForumLikes.CountAsync(l => l.CommentId == commentId);
        return (true, newCount);
    }
    
    
    public async Task<(bool isLiked, int likeCount)> ToggleLikePostForumAsync(int postId, Guid userId)
    {
        var like = await _context.ForumLikes
            .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

        if (like != null)
        {
            _context.ForumLikes.Remove(like);
            await _context.SaveChangesAsync();
            var count = await _context.ForumLikes.CountAsync(l => l.PostId == postId);
            return (false, count);
        }

        _context.ForumLikes.Add(new ForumLike
        {
            PostId = postId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        var newCount = await _context.ForumLikes.CountAsync(l => l.PostId == postId);
        return (true, newCount);
    }
    
    //* Count likes
    public async Task<int> CountLikeOfPostAsync(int postId)
    {
        return await _context.ForumLikes
            .Where(l => l.PostId == postId)
            .CountAsync();
    }
    
    public async Task<int> CountLikeOfCommentAsync(int commentId)
    {
        return await _context.ForumLikes
            .Where(l => l.CommentId == commentId)
            .CountAsync();
    }
    
    public async Task<bool> IsPostLikedByUserAsync(int postId,  Guid userId)
    {
        return await _context.ForumLikes
            .AnyAsync(l => l.PostId == postId && l.UserId == userId);
    }
    
    public async Task<bool> IsCommentLikedByUserAsync(int commentId, Guid userId)
    {
        return await _context.ForumLikes
            .AnyAsync(l => l.CommentId == commentId && l.UserId == userId);
    }
}