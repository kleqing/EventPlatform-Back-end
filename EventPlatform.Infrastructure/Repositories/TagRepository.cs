using EventPlatform.Application.Contracts.Interfaces;
using EventPlatform.Application.Contracts.Requests;
using EventPlatform.Domain.Entities;
using EventPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventPlatform.Infrastructure.Repositories;

public class TagRepository : ITagRepository
{
    private readonly ApplicationDbContext _context;

    public TagRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsUserHaveTag(Guid userId)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(u => u.Tags)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        return user?.Tags.Any() ?? false;
    }

    public async Task<List<Tag>> ListAllTags()
    {
        return await _context.Tags
            .AsNoTracking()
            .OrderBy(t => t.TagName)
            .ToListAsync();
    }

    public async Task<Tag?> CreateTag(CreateTagRequest request)
    {
        var existingTag = await _context.Tags
            .FirstOrDefaultAsync(t => t.TagName == request.TagName);

        if (existingTag != null)
            return existingTag;

        var tag = new Tag
        {
            TagName = request.TagName
        };

        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        return tag;
    }
    
    public async Task<bool> AssignTagsToUser(Guid userId, List<string> tagNames)
    {
        if (tagNames == null || !tagNames.Any())
            return false;

        var user = await _context.Users
            .Include(u => u.Tags)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            return false;

        var tagsInDb = await _context.Tags
            .Where(t => tagNames.Contains(t.TagName))
            .ToListAsync();

        var newTagNames = tagNames.Except(tagsInDb.Select(t => t.TagName)).ToList();

        foreach (var newTagName in newTagNames)
        {
            var newTag = new Tag { TagName = newTagName };
            _context.Tags.Add(newTag);
            tagsInDb.Add(newTag);
        }

        user.Tags.Clear();
        foreach (var tag in tagsInDb)
        {
            user.Tags.Add(tag);
        }

        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<List<Tag>> GetTagsByUser(Guid userId)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(u => u.Tags)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        return user?.Tags.ToList() ?? new List<Tag>();
    }
}