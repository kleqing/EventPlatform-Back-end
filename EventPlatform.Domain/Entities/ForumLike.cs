using System;
using System.Collections.Generic;

namespace EventPlatform.Domain.Entities;

public partial class ForumLike
{
    public Guid LikeId { get; set; }

    public Guid UserId { get; set; }

    public int? PostId { get; set; }

    public int? CommentId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ForumComment? Comment { get; set; }

    public virtual ForumPost? Post { get; set; }
}
