using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlatform.Domain.Entities;

public partial class ForumComment
{
    public int CommentId { get; set; }

    public int PostId { get; set; }

    public Guid UserId { get; set; }

    public int? ParentCommentId { get; set; }

    public string Content { get; set; } = null!;

    public string CommentStatus { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }
    
    [NotMapped]
    public int LikeCount { get; set; }

    public virtual ICollection<ForumLike> ForumLikes { get; set; } = new List<ForumLike>();
    
    public virtual ICollection<ForumComment> InverseParentComment { get; set; } = new List<ForumComment>();

    public virtual ForumComment? ParentComment { get; set; }

    public virtual ForumPost Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
