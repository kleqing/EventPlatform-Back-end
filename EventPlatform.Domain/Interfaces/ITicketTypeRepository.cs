using EventPlatform.Domain.Entities;

namespace EventPlatform.Domain.Interfaces
{
    public interface ITicketTypeRepository
    {
        Task CreateAsync(TicketType ticketType);
        Task CreateRangeAsync(List<TicketType> ticketTypes);
    }
}
