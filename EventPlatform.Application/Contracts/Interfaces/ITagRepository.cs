using EventPlatform.Application.Contracts.Requests;
using EventPlatform.Domain.Entities;

namespace EventPlatform.Application.Contracts.Interfaces;

public interface ITagRepository
{
    Task<bool> IsUserHaveTag(Guid userId);
    Task<List<Tag>> ListAllTags();
    Task<Tag?> CreateTag(CreateTagRequest request);
    Task<bool> AssignTagsToUser(Guid userId, List<string> tagNames);
    Task<List<Tag>> GetTagsByUser(Guid userId);
}