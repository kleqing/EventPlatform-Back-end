using EventPlatform.Application.Common;
using EventPlatform.Application.Contracts.Requests;
using EventPlatform.Application.Interfaces;
using EventPlatform.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EventPlatform.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ForumController : ControllerBase
{
    private readonly IForumRepository _forumRepository;
    
    public ForumController(IForumRepository forumRepository)
    {
        _forumRepository = forumRepository;
    }
    
    [HttpGet("categories")]
    public async Task<IActionResult> ListForumCategories()
    {
        var response = new BaseResultResponse<List<ForumCategory>>();
        var categories = await _forumRepository.ListForumCategoriesAsync();
        
        if (categories.Any())
        {
            response.StatusCode = 200;
            response.Success = true;
            response.Data = categories;
            response.Message = "Forum categories retrieved successfully.";
        }
        else
        {
            response.StatusCode = 404;
            response.Success = false;
            response.Message = "No forum categories found.";
        }
        return Ok(response);
    }

    [HttpGet("forum-title/{postId}")]
    public async Task<IActionResult> DisplayForumTitle(int postId)
    {
        var response = new BaseResultResponse<string>();
        var title = await _forumRepository.DisplayForumTitleAsync(postId);
        
        if (!string.IsNullOrEmpty(title))
        {
            response.StatusCode = 200;
            response.Success = true;
            response.Data = title;
            response.Message = "Forum title retrieved successfully.";
        }
        else
        {
            response.StatusCode = 404;
            response.Success = false;
            response.Message = "Forum not found.";
        }
        return Ok(response);
    }

    [HttpGet("posts")]
    public async Task<IActionResult> ListForumPosts([FromQuery] int? categoryId = null)
    {
        var response = new BaseResultResponse<List<ForumPost>>();
        var posts = await _forumRepository.ListForumPostsAsync(categoryId);

        if (posts.Any())
        {
            response.StatusCode = 200;
            response.Success = true;
            response.Data = posts;
            response.Message = "Forum posts retrieved successfully.";
        }
        else
        {
            response.StatusCode = 404;
            response.Success = false;
            response.Message = "No forum posts found.";
        }

        return Ok(response);
    }

    [HttpGet("post-detail/{postId}")]
    public async Task<IActionResult> GetForumPostDetail(int postId)
    {
        var response = new BaseResultResponse<ForumPost?>();
        var post = await _forumRepository.GetForumPostDetailAsync(postId);
        
        if (post != null)
        {
            response.StatusCode = 200;
            response.Success = true;
            response.Data = post;
            response.Message = "Forum post detail retrieved successfully.";
        }
        else
        {
            response.StatusCode = 404;
            response.Success = false;
            response.Message = "Forum post not found.";
        }
        return Ok(response);
    }

    [HttpPost("create-post")]
    public async Task<IActionResult> CreateForumPost([FromBody] CreateForumPostRequest request)
    {
        var response = new BaseResultResponse<ForumPost>();
        var post = await _forumRepository.CreateForumPostAsync(request);
        
        if (post != null)
        {
            response.StatusCode = 201;
            response.Success = true;
            response.Data = post;
            response.Message = "Forum post created successfully.";
        }
        else
        {
            response.StatusCode = 400;
            response.Success = false;
            response.Message = "Failed to create forum post.";
        }
        return Ok(response);
    }

    [HttpGet("stats/threads-count")]
    public async Task<IActionResult> CountAllForumThreads()
    {
        var response = new BaseResultResponse<int>();
        var count = await _forumRepository.CountAllForumThreadsAsync();
        
        response.StatusCode = 200;
        response.Success = true;
        response.Data = count;
        response.Message = "Total forum threads count retrieved successfully.";
        return Ok(response);
    }

    [HttpGet("stats/members-count")]
    public async Task<IActionResult> CountAllMembers()
    {
        var response = new BaseResultResponse<int>();
        var count = await _forumRepository.CountAllMembersAsync();
        
        response.StatusCode = 200;
        response.Success = true;
        response.Data = count;
        response.Message = "Total forum members count retrieved successfully.";
        return Ok(response);
    }

    [HttpGet("stats/active-threads-count")]
    public async Task<IActionResult> CountAllActiveForumThreads()
    {
        var response = new BaseResultResponse<int>();
        var count = await _forumRepository.CountAllActiveForumThreadAsync();
        
        response.StatusCode = 200;
        response.Success = true;
        response.Data = count;
        response.Message = "Total active forum threads count retrieved successfully.";
        return Ok(response);
    }
    
    [HttpPost("write-comment")]
    public async Task<IActionResult> CreateForumComment([FromBody] CreateForumCommentRequest request)
    {
        var response = new BaseResultResponse<ForumComment>();
        var comment = await _forumRepository.CreateForumCommentAsync(request);
        
        if (comment != null)
        {
            response.StatusCode = 201;
            response.Success = true;
            response.Data = comment;
            response.Message = "Forum comment created successfully.";
        }
        else
        {
            response.StatusCode = 400;
            response.Success = false;
            response.Message = "Failed to create forum comment.";
        }
        return Ok(response);
    }
    
    [HttpGet("comments-count/{postId}")]
    public async Task<IActionResult> CountCommentOfEachListPost(int postId)
    {
        var response = new BaseResultResponse<int>();
        var count = await _forumRepository.CountCommentOfEachListPostAsync(postId);
        
        response.StatusCode = 200;
        response.Success = true;
        response.Data = count;
        response.Message = "Total comments count for the post retrieved successfully.";
        return Ok(response);
    }

    [HttpPost("toggle-like-comment/{commentId}/{userId}")]
    public async Task<IActionResult> ToggleLikeForumComment(int commentId, Guid userId)
    {
        var (isLiked, likeCount) = await _forumRepository.ToggleLikeForumCommentAsync(commentId, userId);

        return Ok(new BaseResultResponse<object>
        {
            StatusCode = 200,
            Success = true,
            Data = new { isLiked, likeCount },
            Message = isLiked ? "Liked comment." : "Unliked comment."
        });
    }

    [HttpPost("toggle-like-post/{postId}/{userId}")]
    public async Task<IActionResult> ToggleLikePostForum(int postId, Guid userId)
    {
        var (isLiked, likeCount) = await _forumRepository.ToggleLikePostForumAsync(postId, userId);

        return Ok(new BaseResultResponse<object>
        {
            StatusCode = 200,
            Success = true,
            Data = new { isLiked, likeCount },
            Message = isLiked ? "Liked post." : "Unliked post."
        });
    }

    [HttpGet("likes-count/{postId}")]
    public async Task<IActionResult> CountLikeOfPost(int postId)
    {
        var response = new BaseResultResponse<int>();
        var count = await _forumRepository.CountLikeOfPostAsync(postId);
        
        response.StatusCode = 200;
        response.Success = true;
        response.Data = count;
        response.Message = "Total likes count for the post retrieved successfully.";
        return Ok(response);
    }
    
    [HttpGet("likes-count/comment/{commentId}")]
    public async Task<IActionResult> CountLikeOfComment(int commentId)
    {
        var count = await _forumRepository.CountLikeOfCommentAsync(commentId);
        return Ok(new BaseResultResponse<int>
        {
            StatusCode = 200,
            Success = true,
            Data = count,
            Message = "Total likes count for the comment retrieved successfully."
        });
    }
    
    [HttpGet("is-liked/post/{postId}/{userId}")]
    public async Task<IActionResult> IsPostLikedByUser(int postId, Guid userId)
    {
        var liked = await _forumRepository.IsPostLikedByUserAsync(postId, userId);
        return Ok(new BaseResultResponse<bool>
        {
            StatusCode = 200,
            Success = true,
            Data = liked,
            Message = liked ? "User liked this post" : "User has not liked this post"
        });
    }

    [HttpGet("is-liked/comment/{commentId}/{userId}")]
    public async Task<IActionResult> IsCommentLikedByUser(int commentId, Guid userId)
    {
        var liked = await _forumRepository.IsCommentLikedByUserAsync(commentId, userId);
        return Ok(new BaseResultResponse<bool>
        {
            StatusCode = 200,
            Success = true,
            Data = liked,
            Message = liked ? "User liked this comment" : "User has not liked this comment"
        });
    }
}