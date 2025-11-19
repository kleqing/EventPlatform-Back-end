using EventPlatform.Application.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventPlatform.Application.Contracts.Requests;
using EventPlatform.Domain.Entities;

namespace EventPlatform.Application.Services.Interfaces.Event
{
    public interface IEventService
    {
        Task<PaginatedResult<EventSummaryDto>> GetEventsAsync(EventQueryParameters query);
        Task<EventDetailDto?> GetEventByIdAsync(int eventId);
        Task CreateEvent(Domain.Entities.Event e);
        Task CreateTicketTypes(List<TicketType> t);
        Task<IEnumerable<EventCategory>> GetAllEventCategories();
        Task<IEnumerable<Domain.Entities.Event>> GetSpeakerEvents(Guid userId);
        Task<AppliedEventsGroupedDto> GetAppliedEventsAsync(Guid userId);
        Task<CommentDto> CreateCommentAsync(CreateFeedbackRequest request);
        Task<List<CommentDto>> ListCommentsAsync(int eventId);
    }
}
