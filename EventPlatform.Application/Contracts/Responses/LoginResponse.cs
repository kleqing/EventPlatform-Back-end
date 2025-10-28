using EventPlatform.Domain.Enums;

namespace EventPlatform.Application.Contracts.Responses;

public class LoginResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Role { get; set; } //UserRole
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}