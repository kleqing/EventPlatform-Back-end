
namespace EventPlatform.Application.Contracts.Requests
{
    public class CreateEventRequest
    {
        public CreateEventDto createEventDto { get; set; }
        public List<CreateTicketTypeDto> createTicketTypeList { get; set; } 
        
    }

    public class CreateEventDto 
    {
        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string EventType { get; set; } = null!;

        public string? Location { get; set; }

        public string? OnlineUrl { get; set; }

        public string EventStatus { get; set; } = null!;

        public string? CoverImageUrl { get; set; }

        public string? CardImageUrl { get; set; }

        public string? OrganizerName { get; set; }

        public string? OrganizerInfo { get; set; }

        public string? OrganizerLogoUrl { get; set; }

        public string? VenueName { get; set; }

        public string? AddressStreet { get; set; }

        public string? AddressWard { get; set; }

        public string? AddressDistrict { get; set; }

        public string? AddressCity { get; set; }

        public int? CategoryId { get; set; }
    }

    public class CreateTicketTypeDto
    {
        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public DateTime SaleStartDate { get; set; }

        public DateTime SaleEndDate { get; set; }
    }
}
