namespace EventPlatform.Application.Contracts.Requests;

public class UpdateSpeakerProfileRequest
{
    public string? Bio { get; set; }
    public string? Topics { get; set; }
    public string? Company { get; set; }
    public string? JobTitle { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? LinkedInUrl { get; set; }
}