using EventPlatform.Application.Contracts.Dtos;
using EventPlatform.Application.Contracts.Requests;
using EventPlatform.Application.Contracts.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPlatform.Application.Services.Interfaces.Event
{
    public interface IRegistrationService
    {
        Task<BookingResponse> CreateBookingAsync(CreateBookingRequest request);
        Task<IEnumerable<UserRegistrationDto>> GetMyRegistrationsAsync();
        Task<JoinMeetingInfoDto> GetJoinMeetingInfoAsync(Guid registrationId);
        Task CancelBookingAsync(CancelBookingRequest request);
    }
}
