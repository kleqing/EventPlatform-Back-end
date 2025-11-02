using EventPlatform.Application.Contracts.Dtos;
using EventPlatform.Application.Services.Interfaces.Event;
using EventPlatform.Application.Contracts.Interfaces;
using EventPlatform.Domain.Entities;

namespace EventPlatform.Infrastructure.Services.Event
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

        public async Task<PaginatedResult<EventSummaryDto>> GetEventsAsync(EventQueryParameters query)
        {
            // Có thể thêm logic nghiệp vụ ở đây (validation, v.v.)
            return await _eventRepository.GetEventsAsync(query);
        }

        public async Task<EventDetailDto?> GetEventByIdAsync(int eventId)
        {
            // Có thể thêm logic kiểm tra quyền hạn, caching, v.v. ở đây
            return await _eventRepository.GetEventByIdAsync(eventId);
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
