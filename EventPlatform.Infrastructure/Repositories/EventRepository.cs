using EventPlatform.Domain.Entities;
using EventPlatform.Domain.Interfaces;
using EventPlatform.Infrastructure.Data;
using EventPlatform.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EventPlatform.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;
        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task CreateAsync(Event e)
        {
            await _context.Events.AddAsync(e);
            await _context.SaveChangesAsync();
        }
    }
}
