using System.Security.Claims;
using EventPlatform.Application.Contracts.Requests;
using EventPlatform.Application.Contracts.Responses;
using EventPlatform.Domain.Entities;

namespace EventPlatform.Application.Services.Interfaces.Auth;

public interface IAuthServices
{
    Task<EventPlatform.Domain.Entities.User> LoginWithGoogle(ClaimsPrincipal claimsPrincipal);
    Task<EventPlatform.Domain.Entities.User?> CreateAccount(RegisterRequest request);
    Task<LoginResponse?> Login(LoginRequest request);
    Task InitiatePasswordReset(string email);
    Task<bool> VerifyPasswordResetToken(string token);
    Task ResetPasswordAsync(ResetPasswordRequest request);
    Task ChangePasswordAsync(EventPlatform.Domain.Entities.User user, ChangePasswordRequest request);
    Task ResendEmailConfirmationAsync(EventPlatform.Domain.Entities.User user);
    Task Logout(EventPlatform.Domain.Entities.User user);
}