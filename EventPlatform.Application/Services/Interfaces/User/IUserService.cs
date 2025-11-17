using System.Threading.Tasks;
using EventPlatform.Application.Contracts.Dtos;
using EventPlatform.Application.Contracts.Requests;

namespace EventPlatform.Application.Services.Interfaces.User;

public interface IUserService
{
    Task<UserProfileDto> GetProfileAsync(string userId);
    Task<UserProfileDto> UpdateProfileAsync(string userId, UpdateUserProfileRequest request);
}
