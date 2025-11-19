using EventPlatform.Application.Common;
using EventPlatform.Application.Contracts.Interfaces;
using EventPlatform.Application.Contracts.Requests;
using EventPlatform.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EventPlatform.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagController : ControllerBase
{ 
    private readonly ITagRepository _tagRepository;
    
    public TagController(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    [HttpGet("list-tags")]
    public async Task<IActionResult> GetAllTags()
    {
        var response = new BaseResultResponse<List<Tag>>();

        var tags = await _tagRepository.ListAllTags();
        
        response.StatusCode = 200;
        response.Success = true;
        response.Data = tags;
        return Ok(response);
    }

    [HttpGet("isUserHaveTag/{userId}")]
    public async Task<IActionResult> IsUserHaveTag(Guid userId)
    {
        var response = new BaseResultResponse<bool>();
        
        var hasTags = await _tagRepository.IsUserHaveTag(userId);
        
        if (!hasTags)
        {
            response.StatusCode = 404;
            response.Success = false;
            response.Message = "User has no tags.";
            return NotFound(response);
        }
        response.StatusCode = 200;
        response.Success = true;
        response.Data = true;
        return Ok(response);
    }

    [HttpPost("create-tag")]
    public async Task<IActionResult> CreateTag([FromBody] CreateTagRequest request)
    {
        var response = new BaseResultResponse<Tag>();
        
        var tag = await _tagRepository.CreateTag(request);
        response.StatusCode = 201;
        response.Success = true;
        response.Data = tag;
        return CreatedAtAction(nameof(CreateTag), response);
    }

    [HttpPost("assign-tags/{userId}")]
    public async Task<IActionResult> AssignTagsToUser(Guid userId, [FromBody] List<string> tagNames)
    {
        var response = new BaseResultResponse<bool>();
        var result = await _tagRepository.AssignTagsToUser(userId, tagNames);

        if (!result)
        {
            response.StatusCode = 400;
            response.Success = false;
            response.Message = "Failed to assign tags to user.";
            return BadRequest(response);
        }

        response.StatusCode = 200;
        response.Success = true;
        response.Data = true;
        return Ok(response);
    }
}