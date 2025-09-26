using EventPlatform.Domain.Entities;

namespace EventPlatform.Application.Services.Interfaces;

public interface IAuthTokenProcess
{
    (string Token, DateTime Expiry) GenerateToken(User user);
    string GenerateRefreshToken();
    void WriteAuthTokenAsHttpOnlyCookie(string cookieName, string token, DateTime expiry);
    void DeleteAuthTokenCookie(string key);
}