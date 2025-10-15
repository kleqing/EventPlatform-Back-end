using System.Security.Claims;
using EventPlatform.Application.Contracts.Requests;
using EventPlatform.Application.Contracts.Responses;
using EventPlatform.Domain.Entities;

namespace EventPlatform.Application.Services.Interfaces.Auth;

public interface IAuthServices
{
    Task<User> LoginWithGoogle(ClaimsPrincipal claimsPrincipal);
    Task<User?> CreateAccount(RegisterRequest request);
    Task<LoginResponse?> Login(LoginRequest request);
    Task InitiatePasswordReset(string email);
    Task<bool> VerifyPasswordResetToken(string token);
    Task ResetPasswordAsync(ResetPasswordRequest request);
    Task ResendEmailConfirmationAsync(User user);
    Task Logout(User user);
}