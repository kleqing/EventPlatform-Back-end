using System.Collections.Generic;

namespace EventPlatform.Application.Contracts.Dtos;

public class AppliedEventsGroupedDto
{
    public List<AppliedEventDto> OfflineEvents { get; set; } = new();
    public List<AppliedEventDto> OnlineEvents { get; set; } = new();
}
