using System;

namespace EventPlatform.Application.Contracts.Dtos
{
    public class JoinMeetingInfoDto
    {
        public Guid RegistrationId { get; set; }
        public Guid UserId { get; set; }
        public int EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string RoomId { get; set; } = string.Empty;
        public string Role { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserIdentifier { get; set; } = string.Empty;
    }
}
