namespace EventPlatform.Application.Contracts.Dtos;

public class SpeakerProfileDto
{
    public string? Bio { get; set; }
    public string? Topics { get; set; }
    public string? Company { get; set; }
    public string? JobTitle { get; set; }
    public string ApprovalStatus { get; set; } = null!;
    public string? WebsiteUrl { get; set; }
    public string? LinkedInUrl { get; set; }
}