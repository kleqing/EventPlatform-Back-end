using System;

namespace EventPlatform.Application.Contracts.Dtos;

public class ConversationDto
{
    public Guid ConversationId { get; set; }
    public ChatUserDto Participant { get; set; } = new();
    public MessageDto? LastMessage { get; set; }
    public int UnreadCount { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
