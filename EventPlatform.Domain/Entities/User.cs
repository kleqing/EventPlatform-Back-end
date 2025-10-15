using System;
using System.Collections.Generic;
using EventPlatform.Domain.Enums;

namespace EventPlatform.Domain.Entities;

public partial class User
{
    public int UserId { get; set; }
    
    public string UserName { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PasswordHash { get; set; }

    public string? AvatarUrl { get; set; }

    public string? PhoneNumber { get; set; }

    public UserRole UserRole { get; set; }

    public string AccountStatus { get; set; } = null!;

    public string? OauthProvider { get; set; }

    public string? OauthProviderId { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? AddressStreet { get; set; }

    public string? AddressWard { get; set; }

    public string? AddressDistrict { get; set; }

    public string? AddressCity { get; set; }

    public virtual ICollection<Connection> ConnectionReceivers { get; set; } = new List<Connection>();

    public virtual ICollection<Connection> ConnectionRequesters { get; set; } = new List<Connection>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<ForumComment> ForumComments { get; set; } = new List<ForumComment>();

    public virtual ICollection<ForumPost> ForumPosts { get; set; } = new List<ForumPost>();

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();

    public virtual SpeakerProfile? SpeakerProfile { get; set; }

    public virtual ICollection<UserInterest> UserInterests { get; set; } = new List<UserInterest>();
}
