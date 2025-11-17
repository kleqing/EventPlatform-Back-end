using EventPlatform.Application.Contracts.Requests;
using EventPlatform.Domain.Entities;
using Events = EventPlatform.Domain.Entities.Event;

namespace EventPlatform.Application.Services.Interfaces.Users;

public interface IUserService
{
    Task<User?> UpdateUserProfile(Guid userId, UpdateUserProfileRequest request);
    Task<User?> ChangePassword(Guid userId, ChangePasswordRequest request);
    Task<List<Events>> ListAppliedUserEventThisMonth(Guid userId);
    Task<SpeakerProfile?> GetSpeakerProfileByUserId(Guid userId);
    Task<SpeakerProfile?> UpdateSpeakerProfile(Guid userId, SpeakerProfile request);
}