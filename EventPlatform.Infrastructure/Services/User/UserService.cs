using System;
using System.Globalization;
using System.Threading.Tasks;
using EventPlatform.Application.Contracts.Dtos;
using EventPlatform.Application.Contracts.Interfaces;
using EventPlatform.Application.Contracts.Requests;
using EventPlatform.Application.Services.Interfaces.User;
using EventPlatform.Shared.Exceptions;

namespace EventPlatform.Infrastructure.Services.User;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserProfileDto> GetProfileAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new GlobalException("User id is required");
        }

        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null)
        {
            throw new GlobalException("User not found");
        }

        return MapToDto(user);
    }

    public async Task<UserProfileDto> UpdateProfileAsync(string userId, UpdateUserProfileRequest request)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new GlobalException("User id is required");
        }

        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null)
        {
            throw new GlobalException("User not found");
        }

        var fullName = request.FullName?.Trim();
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new GlobalException("Full name is required.");
        }

        user.FullName = fullName;
        user.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim();

        if (request.AddressStreet != null)
        {
            user.AddressStreet = string.IsNullOrWhiteSpace(request.AddressStreet)
                ? null
                : request.AddressStreet.Trim();
        }

        user.AddressWard = string.IsNullOrWhiteSpace(request.AddressWard) ? null : request.AddressWard.Trim();
        user.AddressDistrict = string.IsNullOrWhiteSpace(request.AddressDistrict) ? null : request.AddressDistrict.Trim();
        user.AddressCity = string.IsNullOrWhiteSpace(request.AddressCity) ? null : request.AddressCity.Trim();
        user.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(request.DateOfBirth))
        {
            if (!DateOnly.TryParseExact(request.DateOfBirth, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDob))
            {
                throw new GlobalException("Invalid date of birth format. Use yyyy-MM-dd.");
            }

            user.DateOfBirth = parsedDob;
        }
        else
        {
            user.DateOfBirth = null;
        }

        await _userRepository.UpdateAsync(user);

        return MapToDto(user);
    }

    private static UserProfileDto MapToDto(Domain.Entities.User user)
    {
        return new UserProfileDto
        {
            UserId = user.UserId,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            AddressStreet = user.AddressStreet,
            AddressWard = user.AddressWard,
            AddressDistrict = user.AddressDistrict,
            AddressCity = user.AddressCity,
            AvatarUrl = user.AvatarUrl,
            DateOfBirth = user.DateOfBirth?.ToString("yyyy-MM-dd")
        };
    }
}
