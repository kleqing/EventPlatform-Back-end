using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPlatform.Application.Contracts.Requests
{
    public class CancelBookingRequest
    {
        public Guid RegistrationId { get; set; }
        // Cần gửi lại danh sách vé để biết đường cộng lại kho
        public List<TicketSelectionDto> Tickets { get; set; } = new();
    }
}
