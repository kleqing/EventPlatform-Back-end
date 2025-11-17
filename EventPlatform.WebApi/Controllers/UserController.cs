using System.Security.Claims;
using EventPlatform.Application.Common;
using EventPlatform.Application.Contracts.Dtos;
using EventPlatform.Application.Contracts.Interfaces;
using EventPlatform.Application.Contracts.Requests;
using EventPlatform.Application.Services.Interfaces.Auth;
using EventPlatform.Application.Services.Interfaces.Users;
using EventPlatform.Domain.Entities;
//using EventPlatform.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventPlatform.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IAuthServices _authServices;
    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;

    public UserController(IAuthServices authServices, IUserRepository userRepository, IUserService userService)
    {
        _authServices = authServices;
        _userRepository = userRepository;
        _userService = userService;
    }

    //* Don't delete this action, it's used to response to the client that is the user is logged in
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        
        var user = await _userRepository.FindByEmailAsync(userEmail);

        if (user == null)
            return Unauthorized();

        var userDto = new UserDto
        {
            UserId = user.UserId,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            DateOfBirth = user.DateOfBirth,
            AddressWard = user.AddressWard,
            AddressDistrict = user.AddressDistrict,
            AddressCity = user.AddressCity,
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
        };

        return Ok(userDto);
    }

    [Authorize]
    [HttpPut("update-profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileRequest request)
    {
        var response = new BaseResultResponse<User>();
        try
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            var user = await _userRepository.FindByEmailAsync(userEmail);

            if (user == null)
            {
                response.StatusCode = 404;
                response.Message = "User not found.";
                return NotFound(response);
            }

            var updatedUser = await _userService.UpdateUserProfile(user.UserId, request);

            response.StatusCode = 200;
            response.Message = "User profile updated successfully.";
            response.Data = updatedUser;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.StatusCode = 500;
            response.Message = "An error occurred while updating the user profile.";
            return StatusCode(500, response);
        }
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var response = new BaseResultResponse<User>();
        try
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await _userRepository.FindByEmailAsync(userEmail);

            if (user == null)
            {
                response.StatusCode = 404;
                response.Message = "User not found.";
                return NotFound(response);
            }

            var updatePassword = await _userService.ChangePassword(user.UserId, request);
            response.StatusCode = 200;
            response.Message = "Password changed successfully.";
            response.Data = updatePassword;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.StatusCode = 500;
            response.Message = "An error occurred while changing the password.";
            return StatusCode(500, response);
        }
    }

    [Authorize]
    [HttpGet("applied-events-this-month")]
    public async Task<IActionResult> GetAppliedEventsThisMonth()
    {
        var response = new BaseResultResponse<List<EventDto>>();
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var user = await _userRepository.FindByEmailAsync(userEmail);
        if (user == null)
        {
            response.StatusCode = 404;
            response.Message = "User not found.";
            return NotFound(response);
        }

        var events = await _userService.ListAppliedUserEventThisMonth(user.UserId);
        response.StatusCode = 200;
        response.Message = "Applied events retrieved successfully.";
        response.Data = events;
        return Ok(response);
    }

    [Authorize]
    [HttpGet("speaker-profile")]
    public async Task<IActionResult> GetSpeakerProfile()
    {
        var response = new BaseResultResponse<SpeakerProfileDto>();
        
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var user = await _userRepository.FindByEmailAsync(userEmail);
        
        if (user == null)
        {
            response.StatusCode = 404;
            response.Message = "User not found.";
            return NotFound(response);
        }
        var speakerProfile =  await _userService.GetSpeakerProfileByUserId(user.UserId);
        response.StatusCode = 200;
        response.Message = "Speaker profile retrieved successfully.";
        response.Data = speakerProfile;
        return Ok(response);
    }

    [Authorize]
    [HttpPut("update-speaker-profile")]
    public async Task<IActionResult> UpdateSpeakerProfile([FromBody] UpdateSpeakerProfileRequest request)
    {
        var response = new BaseResultResponse<SpeakerProfile>();
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var user = await _userRepository.FindByEmailAsync(userEmail);
        if (user == null)
        {
            response.StatusCode = 404;
            response.Message = "User not found.";
            return NotFound(response);
        }
        var updatedProfile = await _userService.UpdateSpeakerProfile(user.UserId, request);
        if (updatedProfile == null)
        {
            response.StatusCode = 404;
            response.Message = "Speaker profile not found.";
            return NotFound(response);
        }
        else
        {
            response.StatusCode = 200;
            response.Message = "Speaker profile updated successfully.";
            response.Data = updatedProfile;
            return Ok(response);
        }
    }
    
    
}