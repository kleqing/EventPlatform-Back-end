using EventPlatform.Application.Contracts.Dtos;
using EventPlatform.Application.Contracts.Requests;
using EventPlatform.Domain.Entities;

namespace EventPlatform.Application.Services.Interfaces.Users;

public interface IUserService
{
    Task<User?> UpdateUserProfile(Guid userId, UpdateUserProfileRequest request);
    Task<User?> ChangePassword(Guid userId, ChangePasswordRequest request);
    Task<List<EventDto>> ListAppliedUserEventThisMonth(Guid userId);
    Task<SpeakerProfileDto?> GetSpeakerProfileByUserId(Guid userId);
    Task<SpeakerProfile?> UpdateSpeakerProfile(Guid userId, UpdateSpeakerProfileRequest request);
    Task<List<EventDto>> ListRecommendedEventsForUser(Guid userId);
    Task<List<UserDto>> ListRecommendPartnersForUser(Guid userId);
}