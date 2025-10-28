using System;
using System.Collections.Generic;

namespace EventPlatform.Domain.Entities;

public partial class Registration
{
    public Guid RegistrationId { get; set; }

    public Guid UserId { get; set; }

    public int TicketTypeId { get; set; }

    public DateTime? RegistrationDate { get; set; }

    public string UniqueToken { get; set; } = null!;

    public DateTime? CheckInTime { get; set; }

    public virtual TicketType TicketType { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual User User { get; set; } = null!;
}
