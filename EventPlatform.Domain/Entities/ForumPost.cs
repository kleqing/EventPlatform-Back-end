using System;
using System.Collections.Generic;

namespace EventPlatform.Domain.Entities;

public partial class ForumPost
{
    public Guid PostId { get; set; }

    public Guid UserId { get; set; }

    public Guid CategoryId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string PostStatus { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ForumCategory Category { get; set; } = null!;

    public virtual ICollection<ForumComment> ForumComments { get; set; } = new List<ForumComment>();

    public virtual User User { get; set; } = null!;
}
