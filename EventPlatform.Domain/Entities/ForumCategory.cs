using System;
using System.Collections.Generic;

namespace EventPlatform.Domain.Entities;

public partial class ForumCategory
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<ForumPost> ForumPosts { get; set; } = new List<ForumPost>();
}
