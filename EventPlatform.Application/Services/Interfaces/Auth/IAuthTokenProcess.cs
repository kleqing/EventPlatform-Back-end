using EventPlatform.Domain.Entities;

namespace EventPlatform.Application.Services.Interfaces.Auth;

public interface IAuthTokenProcess
{
    (string Token, DateTime Expiry) GenerateToken(EventPlatform.Domain.Entities.User user);
    string GenerateRefreshToken();
    void WriteAuthTokenAsHttpOnlyCookie(string cookieName, string token, DateTime expiry);
    void DeleteAuthTokenCookie(string key);
    Task<string> GenerateEmailConfirmationTokenAsync(EventPlatform.Domain.Entities.User user);
    Task<string> GeneratePasswordTokenResetAsync(EventPlatform.Domain.Entities.User user);
    bool ValidateEmailConfirmationToken(EventPlatform.Domain.Entities.User user, string token);
}