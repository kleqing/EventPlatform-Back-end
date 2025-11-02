using EventPlatform.Domain.Entities;
using EventPlatform.Application.Contracts.Interfaces;
using EventPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventPlatform.Infrastructure.Repositories
{
    public class EventCategoryRepository : IEventCategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public EventCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventCategory>> GetAllEventCategories()
        {
            return await _context.EventCategories.ToListAsync();
        }
    }
}
