using System;
using System.Collections.Generic;

namespace EventPlatform.Domain.Entities;

public partial class UserInterest
{
    public int UserInterestId { get; set; }

    public int UserId { get; set; }

    public string InterestName { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
