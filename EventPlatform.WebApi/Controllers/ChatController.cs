using System.Security.Claims;
using EventPlatform.Application.Common;
using EventPlatform.Application.Contracts.Dtos;
using EventPlatform.Application.Contracts.Requests;
using EventPlatform.Application.Services.Interfaces.Chat;
using EventPlatform.Application.Contracts.Interfaces;
using EventPlatform.Domain.Entities;
using EventPlatform.Shared.Exceptions;
using EventPlatform.WebApi.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace EventPlatform.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly IUserRepository _userRepository;
    private readonly IHubContext<ChatHub> _chatHubContext;

    public ChatController(IChatService chatService, IUserRepository userRepository, IHubContext<ChatHub> chatHubContext)
    {
        _chatService = chatService;
        _userRepository = userRepository;
        _chatHubContext = chatHubContext;
    }

    [HttpGet("users/by-email")]
    public async Task<IActionResult> FindUserByEmail([FromQuery] string email)
    {
        var response = new BaseResultResponse<ChatUserDto> { Success = false };

        try
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                response.StatusCode = 401;
                response.Message = "Unauthorized";
                return Unauthorized(response);
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                response.StatusCode = 400;
                response.Message = "Email is required.";
                return BadRequest(response);
            }

            if (string.Equals(currentUser.Email, email.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                response.StatusCode = 400;
                response.Message = "You cannot start a conversation with yourself.";
                return BadRequest(response);
            }

            var chatUser = await _chatService.FindUserByEmailAsync(currentUser.UserId, email.Trim());
            if (chatUser == null)
            {
                response.StatusCode = 404;
                response.Message = "User not found.";
                return NotFound(response);
            }

            response.StatusCode = 200;
            response.Success = true;
            response.Message = "User found.";
            response.Data = chatUser;
            return Ok(response);
        }
        catch (GlobalException ex)
        {
            response.StatusCode = 400;
            response.Message = ex.Message;
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            response.StatusCode = 500;
            response.Message = ex.Message;
            return StatusCode(500, response);
        }
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> ListConversations()
    {
        var response = new BaseResultResponse<IReadOnlyList<ConversationDto>> { Success = false };
        try
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                response.StatusCode = 401;
                response.Message = "Unauthorized";
                return Unauthorized(response);
            }

            var conversations = await _chatService.ListConversationsAsync(currentUser.UserId);
            response.StatusCode = 200;
            response.Success = true;
            response.Message = "Conversations retrieved.";
            response.Data = conversations;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.StatusCode = 500;
            response.Message = ex.Message;
            return StatusCode(500, response);
        }
    }

    [HttpPost("conversations")]
    public async Task<IActionResult> StartConversation([FromBody] StartConversationRequest request)
    {
        var response = new BaseResultResponse<ConversationDto> { Success = false };

        try
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                response.StatusCode = 401;
                response.Message = "Unauthorized";
                return Unauthorized(response);
            }

            Guid? targetUserId = request.TargetUserId;
            if (!targetUserId.HasValue && !string.IsNullOrWhiteSpace(request.TargetEmail))
            {
                if (string.Equals(currentUser.Email, request.TargetEmail.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    response.StatusCode = 400;
                    response.Message = "You cannot start a conversation with yourself.";
                    return BadRequest(response);
                }

                var targetUser = await _chatService.FindUserByEmailAsync(currentUser.UserId, request.TargetEmail.Trim());
                targetUserId = targetUser?.UserId;
            }

            if (!targetUserId.HasValue)
            {
                response.StatusCode = 400;
                response.Message = "Target user is required.";
                return BadRequest(response);
            }

            var conversation = await _chatService.GetOrCreateConversationAsync(currentUser.UserId, targetUserId.Value);
            response.StatusCode = 200;
            response.Success = true;
            response.Message = "Conversation ready.";
            response.Data = conversation;

            var targetConversation = await _chatService.GetConversationAsync(targetUserId.Value, conversation.ConversationId);
            await _chatHubContext.Clients.User(currentUser.UserId.ToString())
                .SendAsync("ConversationUpdated", conversation);

            if (targetConversation != null)
            {
                await _chatHubContext.Clients.User(targetUserId.Value.ToString())
                    .SendAsync("ConversationUpdated", targetConversation);
            }

            return Ok(response);
        }
        catch (GlobalException ex)
        {
            response.StatusCode = 400;
            response.Message = ex.Message;
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            response.StatusCode = 500;
            response.Message = ex.Message;
            return StatusCode(500, response);
        }
    }

    [HttpGet("conversations/{conversationId:guid}/messages")]
    public async Task<IActionResult> GetMessages(Guid conversationId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var response = new BaseResultResponse<IReadOnlyList<MessageDto>> { Success = false };
        try
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                response.StatusCode = 401;
                response.Message = "Unauthorized";
                return Unauthorized(response);
            }

            var messages = await _chatService.GetMessagesAsync(currentUser.UserId, conversationId, page, pageSize);
            response.StatusCode = 200;
            response.Success = true;
            response.Message = "Messages retrieved.";
            response.Data = messages;
            return Ok(response);
        }
        catch (GlobalException ex)
        {
            response.StatusCode = 400;
            response.Message = ex.Message;
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            response.StatusCode = 500;
            response.Message = ex.Message;
            return StatusCode(500, response);
        }
    }

    [HttpPost("conversations/{conversationId:guid}/messages")]
    public async Task<IActionResult> SendMessage(Guid conversationId, [FromBody] SendMessageRequest request)
    {
        var response = new BaseResultResponse<MessageDto> { Success = false };
        try
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                response.StatusCode = 401;
                response.Message = "Unauthorized";
                return Unauthorized(response);
            }

            var message = await _chatService.SendMessageAsync(currentUser.UserId, conversationId, request);
            response.StatusCode = 200;
            response.Success = true;
            response.Message = "Message sent.";
            response.Data = message;

            var conversation = await _chatService.GetConversationAsync(currentUser.UserId, conversationId);
            if (conversation != null)
            {
                var targetUserId = conversation.Participant.UserId;
                var targetConversation = await _chatService.GetConversationAsync(targetUserId, conversationId);

                await _chatHubContext.Clients.Group(conversationId.ToString())
                    .SendAsync("ReceiveMessage", new
                    {
                        message.MessageId,
                        message.ConversationId,
                        message.SenderId,
                        message.Content,
                        message.MessageType,
                        message.SentAt,
                        message.ReadAt
                    });

                await _chatHubContext.Clients.User(currentUser.UserId.ToString())
                    .SendAsync("ConversationUpdated", conversation);

                if (targetConversation != null)
                {
                    await _chatHubContext.Clients.User(targetUserId.ToString())
                        .SendAsync("ConversationUpdated", targetConversation);
                }
            }

            return Ok(response);
        }
        catch (GlobalException ex)
        {
            response.StatusCode = 400;
            response.Message = ex.Message;
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            response.StatusCode = 500;
            response.Message = ex.Message;
            return StatusCode(500, response);
        }
    }

    private async Task<User?> GetCurrentUserAsync()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return null;
        }

        return await _userRepository.FindByIdAsync(userId.ToString());
    }
}
