using System;
using System.Collections.Generic;

namespace EventPlatform.Domain.Entities;

public partial class Connection
{
    public int ConnectionId { get; set; }

    public Guid RequesterId { get; set; }

    public Guid ReceiverId { get; set; }

    public string ConnectionStatus { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User Receiver { get; set; } = null!;

    public virtual User Requester { get; set; } = null!;
}
