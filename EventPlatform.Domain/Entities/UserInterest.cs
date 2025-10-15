using System;
using System.Collections.Generic;

namespace EventPlatform.Domain.Entities;

public partial class UserInterest
{
    public Guid UserInterestId { get; set; }

    public Guid UserId { get; set; }

    public string InterestName { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
