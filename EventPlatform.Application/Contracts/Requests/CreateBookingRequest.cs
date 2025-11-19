using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPlatform.Application.Contracts.Requests
{
    public class CreateBookingRequest
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        public List<TicketSelectionDto> Tickets { get; set; } = new();

        // Thông tin người đặt (dùng để lưu contact hoặc gửi email sau này)
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
    }

    public class TicketSelectionDto
    {
        public int TicketTypeId { get; set; }
        [Range(1, 10)]
        public int Quantity { get; set; }
    }
}
