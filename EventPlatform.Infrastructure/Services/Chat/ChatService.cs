using EventPlatform.Application.Contracts.Dtos;
using EventPlatform.Application.Contracts.Requests;
using EventPlatform.Application.Services.Interfaces.Chat;
using EventPlatform.Application.Contracts.Interfaces;
using EventPlatform.Infrastructure.Data;
using EventPlatform.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EventPlatform.Infrastructure.Services.Chat;

public class ChatService : IChatService
{
    private readonly ApplicationDbContext _context;
    private readonly IUserRepository _userRepository;

    public ChatService(ApplicationDbContext context, IUserRepository userRepository)
    {
        _context = context;
        _userRepository = userRepository;
    }

    public async Task<ChatUserDto?> FindUserByEmailAsync(Guid currentUserId, string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new GlobalException("chat", "Email is required");
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = await _context.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == normalizedEmail);
        if (user == null || user.UserId == currentUserId)
        {
            return null;
        }

        return new ChatUserDto
        {
            UserId = user.UserId,
            FullName = user.FullName,
            Email = user.Email,
            AvatarUrl = user.AvatarUrl
        };
    }

    public async Task<ConversationDto> GetOrCreateConversationAsync(Guid currentUserId, Guid targetUserId)
    {
        if (currentUserId == targetUserId)
        {
            throw new GlobalException("chat", "You cannot start a conversation with yourself");
        }

        var targetUser = await _userRepository.FindByIdAsync(targetUserId.ToString());
        if (targetUser == null)
        {
            throw new GlobalException("chat", "Target user not found");
        }

        var connection = await _context.Connections
            .Include(c => c.Requester)
            .Include(c => c.Receiver)
            .FirstOrDefaultAsync(c =>
                (c.RequesterId == currentUserId && c.ReceiverId == targetUserId) ||
                (c.RequesterId == targetUserId && c.ReceiverId == currentUserId));

        if (connection == null)
        {
            connection = new Domain.Entities.Connection
            {
                RequesterId = currentUserId,
                ReceiverId = targetUserId,
                ConnectionStatus = "Accepted",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _context.Connections.AddAsync(connection);
            await _context.SaveChangesAsync();
        }
        else if (!string.Equals(connection.ConnectionStatus, "Accepted", StringComparison.OrdinalIgnoreCase))
        {
            connection.ConnectionStatus = "Accepted";
            connection.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        var conversation = await _context.Conversations
            .FirstOrDefaultAsync(c => c.ConnectionId == connection.ConnectionId);

        if (conversation == null)
        {
            conversation = new Domain.Entities.Conversation
            {
                ConversationId = Guid.NewGuid(),
                ConnectionId = connection.ConnectionId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsArchived = false
            };

            await _context.Conversations.AddAsync(conversation);
            await _context.SaveChangesAsync();
        }

        var dto = await ProjectConversationAsync(conversation.ConversationId, currentUserId);
        if (dto == null)
        {
            throw new GlobalException("chat", "Unable to load conversation");
        }

        return dto;
    }

    public async Task<ConversationDto?> GetConversationAsync(Guid currentUserId, Guid conversationId)
    {
        if (!await IsParticipantAsync(currentUserId, conversationId))
        {
            return null;
        }

        return await ProjectConversationAsync(conversationId, currentUserId);
    }

    public async Task<IReadOnlyList<ConversationDto>> ListConversationsAsync(Guid currentUserId)
    {
        var conversations = await _context.Conversations
            .AsNoTracking()
            .Where(c => c.Connection.RequesterId == currentUserId || c.Connection.ReceiverId == currentUserId)
            .OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt)
            .Select(c => new ConversationDto
            {
                ConversationId = c.ConversationId,
                Participant = new ChatUserDto
                {
                    UserId = c.Connection.RequesterId == currentUserId ? c.Connection.Receiver.UserId : c.Connection.Requester.UserId,
                    FullName = c.Connection.RequesterId == currentUserId ? c.Connection.Receiver.FullName : c.Connection.Requester.FullName,
                    Email = c.Connection.RequesterId == currentUserId ? c.Connection.Receiver.Email : c.Connection.Requester.Email,
                    AvatarUrl = c.Connection.RequesterId == currentUserId ? c.Connection.Receiver.AvatarUrl : c.Connection.Requester.AvatarUrl
                },
                LastMessage = c.Messages
                    .OrderByDescending(m => m.SentAt)
                    .Select(m => new MessageDto
                    {
                        MessageId = m.MessageId,
                        ConversationId = m.ConversationId,
                        SenderId = m.SenderId,
                        Content = m.Content,
                        MessageType = m.MessageType,
                        SentAt = m.SentAt,
                        ReadAt = m.ReadAt,
                        IsMine = m.SenderId == currentUserId
                    })
                    .FirstOrDefault(),
                UnreadCount = c.Messages.Count(m => m.ReadAt == null && m.SenderId != currentUserId),
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync();

        return conversations;
    }

    public async Task<IReadOnlyList<MessageDto>> GetMessagesAsync(Guid currentUserId, Guid conversationId, int page = 1, int pageSize = 50)
    {
        if (!await IsParticipantAsync(currentUserId, conversationId))
        {
            throw new GlobalException("chat", "Conversation not found or access denied");
        }

        var skip = Math.Max(page - 1, 0) * Math.Clamp(pageSize, 1, 200);
        var take = Math.Clamp(pageSize, 1, 200);

        var messages = await _context.Messages
            .AsNoTracking()
            .Where(m => m.ConversationId == conversationId)
            .OrderByDescending(m => m.SentAt)
            .Skip(skip)
            .Take(take)
            .Select(m => new MessageDto
            {
                MessageId = m.MessageId,
                ConversationId = m.ConversationId,
                SenderId = m.SenderId,
                Content = m.Content,
                MessageType = m.MessageType,
                SentAt = m.SentAt,
                ReadAt = m.ReadAt,
                IsMine = m.SenderId == currentUserId
            })
            .ToListAsync();

        return messages.OrderBy(m => m.SentAt).ToList();
    }

    public async Task<MessageDto> SendMessageAsync(Guid currentUserId, Guid conversationId, SendMessageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            throw new GlobalException("chat", "Message content is required");
        }

        var conversation = await _context.Conversations
            .Include(c => c.Connection)
            .FirstOrDefaultAsync(c => c.ConversationId == conversationId);

        if (conversation == null)
        {
            throw new GlobalException("chat", "Conversation not found");
        }

        if (conversation.Connection.RequesterId != currentUserId && conversation.Connection.ReceiverId != currentUserId)
        {
            throw new GlobalException("chat", "You are not a participant of this conversation");
        }

        var message = new Domain.Entities.Message
        {
            MessageId = Guid.NewGuid(),
            ConversationId = conversationId,
            SenderId = currentUserId,
            Content = request.Content.Trim(),
            MessageType = string.IsNullOrWhiteSpace(request.MessageType) ? "Text" : request.MessageType,
            SentAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _context.Messages.AddAsync(message);
        conversation.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new MessageDto
        {
            MessageId = message.MessageId,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            Content = message.Content,
            MessageType = message.MessageType,
            SentAt = message.SentAt,
            ReadAt = message.ReadAt,
            IsMine = true
        };
    }

    private async Task<ConversationDto?> ProjectConversationAsync(Guid conversationId, Guid currentUserId)
    {
        return await _context.Conversations
            .AsNoTracking()
            .Where(c => c.ConversationId == conversationId)
            .Select(c => new ConversationDto
            {
                ConversationId = c.ConversationId,
                Participant = new ChatUserDto
                {
                    UserId = c.Connection.RequesterId == currentUserId ? c.Connection.Receiver.UserId : c.Connection.Requester.UserId,
                    FullName = c.Connection.RequesterId == currentUserId ? c.Connection.Receiver.FullName : c.Connection.Requester.FullName,
                    Email = c.Connection.RequesterId == currentUserId ? c.Connection.Receiver.Email : c.Connection.Requester.Email,
                    AvatarUrl = c.Connection.RequesterId == currentUserId ? c.Connection.Receiver.AvatarUrl : c.Connection.Requester.AvatarUrl
                },
                LastMessage = c.Messages
                    .OrderByDescending(m => m.SentAt)
                    .Select(m => new MessageDto
                    {
                        MessageId = m.MessageId,
                        ConversationId = m.ConversationId,
                        SenderId = m.SenderId,
                        Content = m.Content,
                        MessageType = m.MessageType,
                        SentAt = m.SentAt,
                        ReadAt = m.ReadAt,
                        IsMine = m.SenderId == currentUserId
                    })
                    .FirstOrDefault(),
                UnreadCount = c.Messages.Count(m => m.ReadAt == null && m.SenderId != currentUserId),
                UpdatedAt = c.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    private async Task<bool> IsParticipantAsync(Guid userId, Guid conversationId)
    {
        return await _context.Conversations
            .AnyAsync(c => c.ConversationId == conversationId &&
                           (c.Connection.RequesterId == userId || c.Connection.ReceiverId == userId));
    }
}
