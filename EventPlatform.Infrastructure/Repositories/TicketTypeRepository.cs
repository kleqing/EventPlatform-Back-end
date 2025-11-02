using EventPlatform.Domain.Entities;
using EventPlatform.Application.Contracts.Interfaces;
using EventPlatform.Infrastructure.Data;

namespace EventPlatform.Infrastructure.Repositories
{
    public class TicketTypeRepository : ITicketTypeRepository
    {
        private readonly ApplicationDbContext _context;
        public TicketTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task CreateAsync(TicketType ticketType)
        {
            _context.TicketTypes.Add(ticketType);
            await _context.SaveChangesAsync();
        }

        public async Task CreateRangeAsync(List<TicketType> ticketTypes)
        {
            await _context.TicketTypes.AddRangeAsync(ticketTypes);
            await _context.SaveChangesAsync();
        }
    }
}
