using EventPlatform.Application.Contracts.Dtos;
using EventPlatform.Application.Interfaces;
using EventPlatform.Domain.Entities;
using EventPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPlatform.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<EventSummaryDto>> GetEventsAsync(EventQueryParameters queryParams)
        {
            var query = _context.Events.AsQueryable();

            // 1. Lọc (Filtering)
            if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
            {
                query = query.Where(e => e.Title.Contains(queryParams.SearchTerm));
            }

            if (queryParams.CategoryId.HasValue)
            {
                query = query.Where(e => e.CategoryId == queryParams.CategoryId.Value);
            }

            if (queryParams.TagIds != null && queryParams.TagIds.Any())
            {
                query = query.Where(e => e.Tags.Any(t => queryParams.TagIds.Contains(t.TagId)));
            }

            // 2. Đếm tổng số lượng (trước khi phân trang)
            var totalCount = await query.CountAsync();

            // 3. Chiếu (Projection) sang DTO
            var eventsQuery = query
                .OrderByDescending(e => e.StartTime) // Sắp xếp theo sự kiện mới nhất
                .Select(e => new EventSummaryDto
                {
                    EventId = e.EventId,
                    Title = e.Title,
                    CardImageUrl = e.CardImageUrl,
                    StartTime = e.StartTime,
                    Location = e.VenueName + ", " + e.AddressCity, // Cần xử lý null tốt hơn
                    TotalSeats = e.TicketTypes.Sum(t => t.Quantity)
                });

            // 4. Phân trang (Paging)
            var items = await eventsQuery
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            // 5. Trả về kết quả
            return new PaginatedResult<EventSummaryDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize
            };
        }

        public async Task<EventDetailDto?> GetEventByIdAsync(int eventId)
        {
            // Dùng .Select() là cách tốt nhất để tránh Cartesian Explosion
            // EF Core sẽ dịch DTO lồng nhau này thành SQL hiệu quả.
            var eventDetail = await _context.Events
                .Where(e => e.EventId == eventId)
                .Select(e => new EventDetailDto
                {
                    EventId = e.EventId,
                    Title = e.Title,
                    Description = e.Description,
                    CoverImageUrl = e.CoverImageUrl,
                    StartTime = e.StartTime,
                    Location = e.VenueName + ", " + e.AddressCity,
                    TotalSeats = e.TicketTypes.Sum(t => t.Quantity),
                    CategoryName = e.Category.Name,

                    RatingAverage = e.Feedbacks.Any() ? e.Feedbacks.Average(f => f.Rating) : 0,
                    RatingCount = e.Feedbacks.Count(),

                    OrganizerName = e.OrganizerName,
                    OrganizerLogoUrl = e.OrganizerLogoUrl,

                    Speakers = e.Speakers.Select(s => new SpeakerDto
                    {
                        SpeakerId = s.SpeakerId,
                        FullName = s.User.FullName,
                        JobTitle = s.JobTitle,
                        AvatarUrl = s.User.AvatarUrl
                    }).ToList(),

                    Feedbacks = e.Feedbacks.Select(f => new FeedbackDto
                    {
                        FeedbackId = f.FeedbackId,
                        UserId = f.UserId,
                        AuthorName = f.User.FullName,
                        AuthorAvatarUrl = f.User.AvatarUrl,
                        SubmittedAt = f.SubmittedAt ?? DateTime.MinValue,
                        Rating = f.Rating,
                        Comment = f.Comment
                    })
                    .OrderByDescending(f => f.SubmittedAt)
                    .Take(10) // Chỉ lấy 10 feedback mới nhất
                    .ToList()
                })
                .FirstOrDefaultAsync();

            return eventDetail;
        }
    } 
}
