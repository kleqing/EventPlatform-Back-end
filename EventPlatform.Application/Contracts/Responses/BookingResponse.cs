using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPlatform.Application.Contracts.Responses
{
    public class BookingResponse
    {
        public Guid MainRegistrationId { get; set; } // ID đại diện để thanh toán
        public decimal TotalAmount { get; set; }
        public int TotalTickets { get; set; }
        public string PaymentStatus { get; set; }
    }
}
