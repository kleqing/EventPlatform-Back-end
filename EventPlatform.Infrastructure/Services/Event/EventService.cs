using System;
using System.Linq;
using EventPlatform.Application.Contracts.Dtos;
using EventPlatform.Application.Services.Interfaces.Event;
using EventPlatform.Domain.Entities;
using EventPlatform.Application.Contracts.Interfaces;

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
        public async Task<IEnumerable<Domain.Entities.Event>> GetSpeakerEvents(Guid userId)
        {
            return await _eventRepository.GetSpeakerEvents(userId);
        }

        public async Task<AppliedEventsGroupedDto> GetAppliedEventsAsync(Guid userId)
        {
            var events = await _eventRepository.GetAppliedEventsAsync(userId);
            var groupedResult = new AppliedEventsGroupedDto();

            foreach (var evt in events)
            {
                var dto = new AppliedEventDto
                {
                    EventId = evt.EventId,
                    Title = evt.Title,
                    Description = evt.Description,
                    StartTime = evt.StartTime,
                    EndTime = evt.EndTime,
                    Location = evt.Location,
                    OnlineUrl = evt.OnlineUrl,
                    EventStatus = evt.EventStatus
                };

                if (string.Equals(evt.EventType, "Online", StringComparison.OrdinalIgnoreCase))
                {
                    groupedResult.OnlineEvents.Add(dto);
                }
                else
                {
                    groupedResult.OfflineEvents.Add(dto);
                }
            }

            groupedResult.OnlineEvents = groupedResult.OnlineEvents
                .OrderBy(e => e.StartTime)
                .ToList();

            groupedResult.OfflineEvents = groupedResult.OfflineEvents
                .OrderBy(e => e.StartTime)
                .ToList();

            return groupedResult;
        }


    }
}
