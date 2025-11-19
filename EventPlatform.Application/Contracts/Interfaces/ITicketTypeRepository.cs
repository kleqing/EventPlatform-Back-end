using EventPlatform.Domain.Entities;

namespace EventPlatform.Application.Contracts.Interfaces
{
    public interface ITicketTypeRepository
    {
        Task CreateAsync(TicketType ticketType);
        Task CreateRangeAsync(List<TicketType> ticketTypes);
    }
}
