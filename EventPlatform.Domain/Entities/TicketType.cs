using System;
using System.Collections.Generic;

namespace EventPlatform.Domain.Entities;

public partial class TicketType
{
    public int TicketTypeId { get; set; }

    public int EventId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public int AvailableQuantity { get; set; }

    public DateTime SaleStartDate { get; set; }

    public DateTime SaleEndDate { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}
