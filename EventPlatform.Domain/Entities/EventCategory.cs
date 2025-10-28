using System;
using System.Collections.Generic;

namespace EventPlatform.Domain.Entities;

public partial class EventCategory
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
