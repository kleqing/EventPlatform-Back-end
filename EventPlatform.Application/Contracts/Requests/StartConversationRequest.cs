using System;

namespace EventPlatform.Application.Contracts.Requests;

public class StartConversationRequest
{
    public Guid? TargetUserId { get; set; }
    public string? TargetEmail { get; set; }
}
