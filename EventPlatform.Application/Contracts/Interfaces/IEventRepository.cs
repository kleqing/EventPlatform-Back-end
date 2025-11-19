using EventPlatform.Application.Contracts.Dtos;
using EventPlatform.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPlatform.Application.Contracts.Interfaces
{
    public interface IEventRepository
    {
        Task<PaginatedResult<EventSummaryDto>> GetEventsAsync(EventQueryParameters query);
        Task<EventDetailDto?> GetEventByIdAsync(int eventId);
        Task CreateAsync(Event e);
        Task<IEnumerable<Domain.Entities.Event>> GetSpeakerEvents(Guid userId);
        Task<IEnumerable<Domain.Entities.Event>> GetAppliedEventsAsync(Guid userId);
        Task AddFeedbackAsync(Feedback feedback);
        Task<List<Feedback>> GetFeedbacksByEventIdAsync(int eventId);
    }
}
