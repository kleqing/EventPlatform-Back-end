using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPlatform.Application.Contracts.Dtos
{
    public class EventSummaryDto
    {
        public int EventId { get; set; }
        public string Title { get; set; }
        public string CardImageUrl { get; set; }
        public DateTime StartTime { get; set; }
        public string Location { get; set; }
        public int TotalSeats { get; set; }
    }
}
