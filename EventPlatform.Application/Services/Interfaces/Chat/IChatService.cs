using EventPlatform.Application.Contracts.Dtos;
using EventPlatform.Application.Contracts.Requests;

namespace EventPlatform.Application.Services.Interfaces.Chat;

public interface IChatService
{
    Task<ChatUserDto?> FindUserByEmailAsync(Guid currentUserId, string email);
    Task<ConversationDto> GetOrCreateConversationAsync(Guid currentUserId, Guid targetUserId);
    Task<ConversationDto?> GetConversationAsync(Guid currentUserId, Guid conversationId);
    Task<IReadOnlyList<ConversationDto>> ListConversationsAsync(Guid currentUserId);
    Task<IReadOnlyList<MessageDto>> GetMessagesAsync(Guid currentUserId, Guid conversationId, int page = 1, int pageSize = 50);
    Task<MessageDto> SendMessageAsync(Guid currentUserId, Guid conversationId, SendMessageRequest request);
}
