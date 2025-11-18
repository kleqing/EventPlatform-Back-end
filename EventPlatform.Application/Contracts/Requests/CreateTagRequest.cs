namespace EventPlatform.Application.Contracts.Requests;

public class CreateTagRequest
{
    public Guid UserId { get; set; }
    public string TagName { get; set; } = string.Empty;
}