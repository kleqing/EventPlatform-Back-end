using EventPlatform.Application.Contracts.Dtos;
using EventPlatform.Application.Interfaces;
using EventPlatform.Application.Services.Interfaces.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPlatform.Infrastructure.Services.Event
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
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
    }
}
