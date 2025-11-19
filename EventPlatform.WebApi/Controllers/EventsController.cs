using System.Collections.Generic;
using System.Security.Claims;
using EventPlatform.Application.Common;
using EventPlatform.Application.Contracts.Dtos;
using EventPlatform.Application.Contracts.Requests;
using EventPlatform.Application.Services.Interfaces.Event;
using EventPlatform.Domain.Entities;
using EventPlatform.Shared.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventPlatform.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public EventsController(IEventService eventService, HttpClient httpClient, IConfiguration configuration)
        {
            _eventService = eventService;
            _httpClient = httpClient;
            _configuration = configuration;
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

        [Authorize]
        [HttpPost("create")]
        public async Task<ActionResult> CreateEvent([FromBody] CreateEventRequest createEventRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return BadRequest();
            var userGuidId = Guid.Parse(userId);
            if (userGuidId == Guid.Empty) return BadRequest();
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
                // Lấy Base URL từ cấu hình
                var fastApiBaseUrl = UrlHelper.GetFastAPIUrl(_configuration); ;

                // Dựng URL cho action UPSERT
                var fastApiUrl = $"{fastApiBaseUrl}/internal/events/manage/{newEvent.EventId}?action=UPSERT";

                // Gửi yêu cầu POST (không cần body vì FastAPI sẽ tự lấy data từ CSDL)
                var response = await _httpClient.PostAsync(fastApiUrl, null);
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

        [HttpGet("getSpeakerEvents")]
        public async Task<ActionResult> GetSpeakerEvents()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return BadRequest();
            var userGuidId = Guid.Parse(userId);
            if (userGuidId == Guid.Empty) return BadRequest();

            try
            {
                var events = await _eventService.GetSpeakerEvents(userGuidId);
                var eventsDto = events.Select(e => new MyEventDto
                {
                    EventId = e.EventId,
                    Description = e.Description,
                    CoverImageUrl = e.CoverImageUrl,
                    Title = e.Title,
                    TotalSeats = e.TicketTypes.Sum(t => t.Quantity),
                    StartTime = e.StartTime,
                    Location = e.VenueName,
                }).ToList();
                return Ok(eventsDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [Authorize]
        [HttpGet("applied")]
        public async Task<IActionResult> GetAppliedEvents()
        {
            var response = new BaseResultResponse<AppliedEventsGroupedDto>();

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    response.Success = false;
                    response.Message = "User not found in token.";
                    return StatusCode(StatusCodes.Status401Unauthorized, response);
                }

                var userGuidId = Guid.Parse(userId);
                var appliedEvents = await _eventService.GetAppliedEventsAsync(userGuidId);

                response.StatusCode = StatusCodes.Status200OK;
                response.Success = true;
                response.Message = "Applied events retrieved successfully.";
                response.Data = appliedEvents;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Success = false;
                response.Message = "An error occurred while processing your request.";
                response.Errors = new List<string> { ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [AllowAnonymous]
        [HttpGet("listComments/{eventId}")]
        public async Task<IActionResult> ListComments(int eventId)
        {
            var response = new BaseResultResponse<List<CommentDto>>();
            
            var comments = await _eventService.ListCommentsAsync(eventId);

            if (comments == null)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Success = false;
                response.Message = "No comments found for the specified event.";
                return NotFound(response);
            }
            
            response.StatusCode = StatusCodes.Status200OK;
            response.Success = true;
            response.Message = "Comments retrieved successfully.";
            response.Data = comments;
            return Ok(response);
        }

        [Authorize]
        [HttpPost("createComment")]
        public async Task<IActionResult> CreateComment([FromBody] CreateFeedbackRequest request)
        {
            var response = new BaseResultResponse<CommentDto>();
            var createComment = await _eventService.CreateCommentAsync(request);
            
            if (createComment == null)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Success = false;
                response.Message = "Failed to create comment.";
                return BadRequest(response);
            }
            
            response.StatusCode = StatusCodes.Status200OK;
            response.Success = true;
            response.Message = "Comment created successfully.";
            response.Data = createComment;
            return Ok(response);
        }
    }
}
