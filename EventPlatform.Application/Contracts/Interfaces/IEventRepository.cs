using EventPlatform.Application.Contracts.Dtos;
using EventPlatform.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPlatform.Application.Interfaces
{
    public interface IEventRepository
    {
        Task<PaginatedResult<EventSummaryDto>> GetEventsAsync(EventQueryParameters query);
        Task<EventDetailDto?> GetEventByIdAsync(int eventId);
    }
}
