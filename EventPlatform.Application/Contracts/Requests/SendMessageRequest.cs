using System;

namespace EventPlatform.Application.Contracts.Requests;

public class SendMessageRequest
{
    public string Content { get; set; } = string.Empty;
    public string MessageType { get; set; } = "Text";
}
