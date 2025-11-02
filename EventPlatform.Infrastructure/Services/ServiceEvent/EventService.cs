using EventPlatform.Application.Services.Interfaces.ServiceEvent;
using EventPlatform.Domain.Entities;
using EventPlatform.Domain.Interfaces;

namespace EventPlatform.Infrastructure.Services.ServiceEvent
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly ITicketTypeRepository _ticketTypeRepository;
        private readonly IEventCategoryRepository _eventCategoryRepository;
        public EventService(IEventRepository eventRepository, ITicketTypeRepository ticketTypeRepository, IEventCategoryRepository eventCategoryRepository)
        {
            _eventRepository = eventRepository;
            _ticketTypeRepository = ticketTypeRepository;
            _eventCategoryRepository = eventCategoryRepository;
        }
        public async Task CreateEvent(Domain.Entities.Event e)
        {
            await _eventRepository.CreateAsync(e);
        }

        public async Task CreateTicketTypes(List<TicketType> t)
        {
            await _ticketTypeRepository.CreateRangeAsync(t);
        }

        public async Task<IEnumerable<EventCategory>> GetAllEventCategories()
        {
            return await _eventCategoryRepository.GetAllEventCategories();
            
        }
    }
}
