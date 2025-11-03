namespace EventPlatform.Application.Contracts.Dtos
{
    public class MyEventDto
    {
        public int EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CoverImageUrl { get; set; }
        public DateTime StartTime { get; set; }
        public string Location { get; set; }
        public int TotalSeats { get; set; }
    }
}
