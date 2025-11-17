using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using EventPlatform.Application.Common;
using EventPlatform.Application.Contracts.Dtos;
using EventPlatform.Application.Contracts.Requests;
using EventPlatform.Application.Services.Interfaces.Auth;
using EventPlatform.Application.Services.Interfaces.User;
using EventPlatform.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventPlatform.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthServices _authServices;


    public UserController(IAuthServices authServices, IUserService userService)
    {
        _authServices = authServices;
        _userService = userService;
    }

    //* Don't delete this action, it's used to response to the client that is the user is logged in
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        return Ok(new { email = userEmail });
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var response = new BaseResultResponse<UserProfileDto>();

        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                response.StatusCode = StatusCodes.Status401Unauthorized;
                response.Success = false;
                response.Message = "User not found in token.";
                response.Data = null;
                return StatusCode(StatusCodes.Status401Unauthorized, response);
            }

            var profile = await _userService.GetProfileAsync(userId);
            response.StatusCode = StatusCodes.Status200OK;
            response.Success = true;
            response.Message = "Profile retrieved successfully.";
            response.Data = profile;
            return Ok(response);
        }
        catch (GlobalException e)
        {
            response.StatusCode = StatusCodes.Status400BadRequest;
            response.Success = false;
            response.Message = e.Message;
            response.Errors = new List<string> { e.Message };
            response.Data = null;
            return BadRequest(response);
        }
        catch (Exception e)
        {
            response.StatusCode = StatusCodes.Status500InternalServerError;
            response.Success = false;
            response.Message = "An error occurred while processing your request.";
            response.Errors = new List<string> { e.Message };
            response.Data = null;
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileRequest request)
    {
        var response = new BaseResultResponse<UserProfileDto>();

        if (!ModelState.IsValid)
        {
            response.StatusCode = StatusCodes.Status400BadRequest;
            response.Success = false;
            response.Message = "Invalid profile data.";
            response.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            response.Data = null;
            return BadRequest(response);
        }

        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                response.StatusCode = StatusCodes.Status401Unauthorized;
                response.Success = false;
                response.Message = "User not found in token.";
                response.Data = null;
                return StatusCode(StatusCodes.Status401Unauthorized, response);
            }

            var profile = await _userService.UpdateProfileAsync(userId, request);
            response.StatusCode = StatusCodes.Status200OK;
            response.Success = true;
            response.Message = "Profile updated successfully.";
            response.Data = profile;
            return Ok(response);
        }
        catch (GlobalException e)
        {
            response.StatusCode = StatusCodes.Status400BadRequest;
            response.Success = false;
            response.Message = e.Message;
            response.Errors = new List<string> { e.Message };
            response.Data = null;
            return BadRequest(response);
        }
        catch (Exception e)
        {
            response.StatusCode = StatusCodes.Status500InternalServerError;
            response.Success = false;
            response.Message = "An error occurred while processing your request.";
            response.Errors = new List<string> { e.Message };
            response.Data = null;
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }
}