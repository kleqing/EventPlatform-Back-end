using EventPlatform.Application.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPlatform.Application.Services.Interfaces.Event
{
    public interface IEventService
    {
        Task<PaginatedResult<EventSummaryDto>> GetEventsAsync(EventQueryParameters query);
        Task<EventDetailDto?> GetEventByIdAsync(int eventId);
    }
}
