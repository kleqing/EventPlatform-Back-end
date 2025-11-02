
using EventPlatform.Domain.Entities;

namespace EventPlatform.Application.Services.Interfaces.ServiceEvent
{
    public interface IEventService
    {
        Task CreateEvent(Event e);
        Task CreateTicketTypes(List<TicketType> t);
        Task<IEnumerable<EventCategory>> GetAllEventCategories();
    }
}
