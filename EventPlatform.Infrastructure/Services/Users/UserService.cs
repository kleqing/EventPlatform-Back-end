using EventPlatform.Application.Contracts.Interfaces;
using EventPlatform.Application.Contracts.Requests;
using EventPlatform.Application.Services.Interfaces.Users;
using EventPlatform.Domain.Entities;
using EventPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Events = EventPlatform.Domain.Entities.Event;

namespace EventPlatform.Infrastructure.Services.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly ApplicationDbContext _context;

    public UserService(IUserRepository repository, ApplicationDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<User?> UpdateUserProfile(Guid userId, UpdateUserProfileRequest request)
    {
        var isUserExist = await _repository.FindByIdAsync(userId.ToString());

        if (isUserExist != null)
        {
            isUserExist.FullName = request.FullName;
            isUserExist.PhoneNumber = request.PhoneNumber;
            isUserExist.DateOfBirth = request.DateOfBirth;
            isUserExist.AddressWard = request.AddressWard;
            isUserExist.AddressDistrict = request.AddressDistrict;
            isUserExist.AddressCity = request.AddressCity;
            isUserExist.Email = request.Email;
            await _repository.UpdateAsync(isUserExist);
            return isUserExist;
        }
        return null;
    }

    public async Task<User?> ChangePassword(Guid userId, ChangePasswordRequest request)
    {
        var user = await _repository.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new Exception("User not found");

        var isCurrentPasswordValid = await _repository.CheckPasswordAsync(user, request.CurrentPassword);
        if (!isCurrentPasswordValid)
            throw new Exception("Current password is incorrect");

        if (request.NewPassword != request.ConfirmNewPassword)
            throw new Exception("New password and confirm password do not match");

        var updatedUser = await _repository.ResetPasswordAsync(user, request.NewPassword);
        return updatedUser; 
    }

    public async Task<List<Events>> ListAppliedUserEventThisMonth(Guid userId)
    {
        var now = DateTime.Now;

        var events = await _context.Registrations
            .Where(r => r.UserId == userId)
            .Select(r => r.TicketType.Event)
            .Where(e => e.StartTime.Month == now.Month && e.StartTime.Year == now.Year)
            .ToListAsync();

        return events;
    }
    
    public async Task<SpeakerProfile?> GetSpeakerProfileByUserId(Guid userId)
    {
        var speakerProfile = await _context.SpeakerProfiles
            .FirstOrDefaultAsync(sp => sp.UserId == userId);

        return speakerProfile;
    }
    
    public async Task<SpeakerProfile?> UpdateSpeakerProfile(Guid userId, SpeakerProfile request)
    {
        var existingProfile = await _context.SpeakerProfiles
            .FirstOrDefaultAsync(sp => sp.UserId == userId);

        if (existingProfile == null)
        {
            return null;
        }

        existingProfile.Bio = request.Bio;
        existingProfile.Topics = request.Topics;
        existingProfile.Company = request.Company;
        existingProfile.JobTitle = request.JobTitle;
        existingProfile.WebsiteUrl = request.WebsiteUrl;
        existingProfile.LinkedInUrl = request.LinkedInUrl;

        existingProfile.User.UpdatedAt = DateTime.UtcNow;

        _context.SpeakerProfiles.Update(existingProfile);
        
        await _context.SaveChangesAsync();

        return existingProfile;
    }
}