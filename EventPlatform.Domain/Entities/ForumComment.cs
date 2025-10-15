using System;
using System.Collections.Generic;

namespace EventPlatform.Domain.Entities;

public partial class ForumComment
{
    public int CommentId { get; set; }

    public int PostId { get; set; }

    public int UserId { get; set; }

    public int? ParentCommentId { get; set; }

    public string Content { get; set; } = null!;

    public string CommentStatus { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ForumComment> InverseParentComment { get; set; } = new List<ForumComment>();

    public virtual ForumComment? ParentComment { get; set; }

    public virtual ForumPost Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
