using System.Security.Claims;
using EventPlatform.Application.Common;
using EventPlatform.Application.Contracts.Requests;
using EventPlatform.Application.Services.Interfaces.Auth;
using EventPlatform.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventPlatform.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IAuthServices _authServices;
    
    public UserController(IAuthServices authServices)
    {
        _authServices = authServices;
    }

    //* Don't delete this action, it's used to response to the client that is the user is logged in
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        return Ok(new { email = userEmail });
    }
}