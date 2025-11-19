using System;
using System.Collections.Generic;

namespace EventPlatform.Domain.Entities;

public partial class Conversation
{
    public Guid ConversationId { get; set; }

    public int ConnectionId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsArchived { get; set; }

    public virtual Connection Connection { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
