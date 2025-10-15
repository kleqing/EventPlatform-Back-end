using System;
using System.Collections.Generic;

namespace EventPlatform.Domain.Entities;

public partial class ForumComment
{
    public Guid CommentId { get; set; }

    public Guid PostId { get; set; }

    public Guid UserId { get; set; }

    public Guid? ParentCommentId { get; set; }

    public string Content { get; set; } = null!;

    public string CommentStatus { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ForumComment> InverseParentComment { get; set; } = new List<ForumComment>();

    public virtual ForumComment? ParentComment { get; set; }

    public virtual ForumPost Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
