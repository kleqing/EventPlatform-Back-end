using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPlatform.Application.Contracts.Dtos
{
    // DTO cho trang chi tiết event
    public class EventDetailDto
    {
        public int EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CoverImageUrl { get; set; }
        public DateTime StartTime { get; set; }
        public string Location { get; set; }
        public int TotalSeats { get; set; }

        public string CategoryName { get; set; }
        public double RatingAverage { get; set; }
        public int RatingCount { get; set; }

        public string OrganizerName { get; set; }
        public string OrganizerLogoUrl { get; set; }

        public List<SpeakerDto> Speakers { get; set; } = new();
        public List<FeedbackDto> Feedbacks { get; set; } = new();
    }

    public class SpeakerDto
    {
        public int SpeakerId { get; set; }
        public string FullName { get; set; }
        public string JobTitle { get; set; }
        public string AvatarUrl { get; set; }
    }

    public class FeedbackDto
    {
        public int FeedbackId { get; set; }
        public Guid UserId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorAvatarUrl { get; set; }
        public DateTime SubmittedAt { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
