namespace EventPlatform.Application.Contracts.Requests;

public class LoginRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}