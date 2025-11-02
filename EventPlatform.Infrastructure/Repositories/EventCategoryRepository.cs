

using EventPlatform.Domain.Entities;
using EventPlatform.Domain.Interfaces;
using EventPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;

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
