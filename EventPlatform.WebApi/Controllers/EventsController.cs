using EventPlatform.Application.Contracts.Dtos;
using EventPlatform.Application.Services.Interfaces.Event;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventPlatform.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        /// <param name="query">Tham số tìm kiếm, lọc, phân trang</param>
        /// <returns>Danh sách sự kiện tóm tắt</returns>
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<EventSummaryDto>>> GetEvents(
            [FromQuery] EventQueryParameters query)
        {
            var result = await _eventService.GetEventsAsync(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventDetailDto>> GetEvent(int id)
        {
            var result = await _eventService.GetEventByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
