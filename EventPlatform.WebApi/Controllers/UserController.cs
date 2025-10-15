using EventPlatform.Application.Common;
using EventPlatform.Application.Contracts.Requests;
using EventPlatform.Application.Services.Interfaces.Auth;
using EventPlatform.Domain.Entities;
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

    // [HttpPost("register")]
    // public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    // {
    //     var response = new BaseResultResponse<User>();
    //     try
    //     {
    //         var user = await _authServices.Register(request);
    //         response.StatusCode = 200;
    //         response.Success = true;
    //         response.Message = "User registered successfully.";
    //         response.Data = user;
    //         return Ok(response);
    //     }
    //     catch (Exception ex)
    //     {
    //         response.StatusCode = 400;
    //         response.Success = false;
    //         response.Message = ex.Message;
    //         return BadRequest(response);
    //     }
    // }
}