using System;
using System.Linq;
using EventPlatform.Application.Contracts.Dtos;
using EventPlatform.Application.Services.Interfaces.Event;
using EventPlatform.Domain.Entities;
using EventPlatform.Application.Contracts.Interfaces;
using EventPlatform.Application.Contracts.Requests;

namespace EventPlatform.Infrastructure.Services.Event
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly ITicketTypeRepository _ticketTypeRepository;
        private readonly IEventCategoryRepository _eventCategoryRepository;
        private readonly IUserRepository _userRepository;

        public EventService(IEventRepository eventRepository, ITicketTypeRepository ticketTypeRepository, IEventCategoryRepository eventCategoryRepository, IUserRepository userRepository)
        {
            _eventRepository = eventRepository;
            _ticketTypeRepository = ticketTypeRepository;
            _eventCategoryRepository = eventCategoryRepository;
            _userRepository = userRepository;
        }

        public async Task<PaginatedResult<EventSummaryDto>> GetEventsAsync(EventQueryParameters query)
        {
            // Có thể thêm logic nghiệp vụ ở đây (validation, v.v.)
            return await _eventRepository.GetEventsAsync(query);
        }

        public async Task<EventDetailDto?> GetEventByIdAsync(int eventId)
        {
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

        public async Task<CommentDto> CreateCommentAsync(CreateFeedbackRequest request)
        {
            var user = await _userRepository.FindByEmailAsync(request.UserEmail);
            
            if (user == null)
            {
                throw new Exception("User not found");
            }
            
            var feedback = new Feedback
            {
                EventId = request.EventId,
                UserId = user.UserId,
                Rating = request.Rating,
                Comment = request.Comment,
                SubmittedAt = request.SubmittedAt ?? DateTime.UtcNow
            };

            await _eventRepository.AddFeedbackAsync(feedback);

            return new CommentDto
            {
                UserName = feedback.User.FullName,
                SubmittedAt = feedback.SubmittedAt,
                Rating = feedback.Rating,
                Comment = feedback.Comment,
                AvatarUrl = feedback.User.AvatarUrl
            };
        }
        
        public async Task<List<CommentDto>> ListCommentsAsync(int eventId)
        {
            var feedbacks = await _eventRepository.GetFeedbacksByEventIdAsync(eventId);
            
            return feedbacks.Select(f => new CommentDto
            {
                UserName = f.User.FullName,
                SubmittedAt = f.SubmittedAt,
                Rating = f.Rating,
                Comment = f.Comment,
                AvatarUrl = f.User.AvatarUrl
            }).ToList();
        }
    }
}
