using EventPlatform.Domain.Entities;

namespace EventPlatform.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> FindByEmailAsync(string email);
    Task<User?> FindByIdAsync(string userId);
    Task<User?> FindByNameAsync(string fullName);
    Task CreateAsync(User user);
    Task CreateAsync(User user, string password);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
    Task<User?> AddToRoleAsync(User user, string roleName);
    Task<User?> ConfirmEmailAsync(User user);
    Task<bool> CheckPasswordAsync(User user, string password);
    Task<bool> IsEmailConfirmedAsync(User user);
    Task<User?> ResetPasswordAsync(User user, string newPassword);
}