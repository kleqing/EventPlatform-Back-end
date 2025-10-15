namespace EventPlatform.Application.Contracts.Responses;

public class LoginResponse
{
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}