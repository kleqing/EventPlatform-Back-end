using EventPlatform.Application.Contracts.Requests;
using EventPlatform.Application.Services.Interfaces.ServiceEvent;
using EventPlatform.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace EventPlatform.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : Controller
    {
        private readonly IEventService _eventService;
        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<ActionResult> CreateEvent([FromBody] CreateEventRequest createEventRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return BadRequest();
            var userGuidId = Guid.Parse(userId);
            if(userGuidId == Guid.Empty) return BadRequest();
            var createEventDto = createEventRequest.createEventDto;
            var createTicketTypeList = createEventRequest.createTicketTypeList;   
            var newEvent = new Event
            {
                Title = createEventDto.Title,
                Description = createEventDto.Description,
                StartTime = createEventDto.StartTime,
                EndTime = createEventDto.EndTime,
                EventType = createEventDto.EventType,
                Location = createEventDto.Location,
                OnlineUrl = createEventDto.OnlineUrl ?? null,
                EventStatus = createEventDto.EventStatus,
                CreatedByUserId = userGuidId,
                CreatedAt = new DateTime(),
                CoverImageUrl = createEventDto.CoverImageUrl,
                CardImageUrl = createEventDto.CardImageUrl,
                OrganizerInfo = createEventDto.OrganizerInfo,
                OrganizerLogoUrl = createEventDto.OrganizerLogoUrl,
                VenueName = createEventDto.VenueName,
                AddressStreet = createEventDto.AddressStreet,
                AddressWard = createEventDto.AddressWard,
                AddressDistrict = createEventDto.AddressDistrict,
                AddressCity = createEventDto.AddressCity,
                CategoryId = createEventDto.CategoryId
            };
            try
            {
                await _eventService.CreateEvent(newEvent);
                var newTicketTypes = createTicketTypeList.Select(t => new TicketType
                {
                    EventId = newEvent.EventId,
                    Name = t.Name,
                    Price = t.Price,
                    Quantity = t.Quantity,
                    AvailableQuantity = t.Quantity,
                    SaleStartDate = t.SaleStartDate,
                    SaleEndDate = t.SaleEndDate,
                }).ToList();
                await _eventService.CreateTicketTypes(newTicketTypes);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }

        [HttpGet("getAllEventCategories")]
        public async Task<ActionResult> GetAllEventCategories()
        {
            try
            {
                var eventCategories = await _eventService.GetAllEventCategories();
                return Ok(eventCategories);
            }
            catch (Exception ex)
            {
                return BadRequest(ex); 
            }

        }

    }
}
