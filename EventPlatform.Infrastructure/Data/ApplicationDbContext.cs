using System;
using System.Collections.Generic;
using EventPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventPlatform.Infrastructure.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Connection> Connections { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventCategory> EventCategories { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<ForumCategory> ForumCategories { get; set; }

    public virtual DbSet<ForumComment> ForumComments { get; set; }

    public virtual DbSet<ForumPost> ForumPosts { get; set; }

    public virtual DbSet<Registration> Registrations { get; set; }

    public virtual DbSet<SpeakerProfile> SpeakerProfiles { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<TicketType> TicketTypes { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> User { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Connection>(entity =>
        {
            entity.HasKey(e => e.ConnectionId).HasName("PK__Connecti__404A64F34DA27EFE");

            entity.HasIndex(e => new { e.RequesterId, e.ReceiverId }, "UQ__Connecti__63A81B34272D305D").IsUnique();

            entity.Property(e => e.ConnectionId).HasColumnName("ConnectionID");
            entity.Property(e => e.ConnectionStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.ReceiverId).HasColumnName("ReceiverID");
            entity.Property(e => e.RequesterId).HasColumnName("RequesterID");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Receiver).WithMany(p => p.ConnectionReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Connectio__Recei__1CBC4616");

            entity.HasOne(d => d.Requester).WithMany(p => p.ConnectionRequesters)
                .HasForeignKey(d => d.RequesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Connectio__Reque__1BC821DD");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Events__7944C87090AE50A7");

            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.AddressCity)
                .HasMaxLength(100)
                .HasColumnName("Address_City");
            entity.Property(e => e.AddressDistrict)
                .HasMaxLength(100)
                .HasColumnName("Address_District");
            entity.Property(e => e.AddressStreet)
                .HasMaxLength(255)
                .HasColumnName("Address_Street");
            entity.Property(e => e.AddressWard)
                .HasMaxLength(100)
                .HasColumnName("Address_Ward");
            entity.Property(e => e.CardImageUrl)
                .HasMaxLength(500)
                .HasColumnName("CardImageURL");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CoverImageUrl)
                .HasMaxLength(500)
                .HasColumnName("CoverImageURL");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserID");
            entity.Property(e => e.EventStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Draft");
            entity.Property(e => e.EventType).HasMaxLength(10);
            entity.Property(e => e.Location).HasMaxLength(500);
            entity.Property(e => e.OnlineUrl)
                .HasMaxLength(500)
                .HasColumnName("OnlineURL");
            entity.Property(e => e.OrganizerLogoUrl)
                .HasMaxLength(500)
                .HasColumnName("OrganizerLogoURL");
            entity.Property(e => e.OrganizerName).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.VenueName).HasMaxLength(255);

            entity.HasOne(d => d.Category).WithMany(p => p.Events)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Events__Category__619B8048");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.Events)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Events__CreatedB__60A75C0F");

            entity.HasMany(d => d.Speakers).WithMany(p => p.Events)
                .UsingEntity<Dictionary<string, object>>(
                    "EventSpeaker",
                    r => r.HasOne<SpeakerProfile>().WithMany()
                        .HasForeignKey("SpeakerId")
                        .HasConstraintName("FK__EventSpea__Speak__656C112C"),
                    l => l.HasOne<Event>().WithMany()
                        .HasForeignKey("EventId")
                        .HasConstraintName("FK__EventSpea__Event__6477ECF3"),
                    j =>
                    {
                        j.HasKey("EventId", "SpeakerId").HasName("PK__EventSpe__FEDABD03355441C9");
                        j.ToTable("EventSpeakers");
                        j.IndexerProperty<int>("EventId").HasColumnName("EventID");
                        j.IndexerProperty<int>("SpeakerId").HasColumnName("SpeakerID");
                    });

            entity.HasMany(d => d.Tags).WithMany(p => p.Events)
                .UsingEntity<Dictionary<string, object>>(
                    "EventTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .HasConstraintName("FK__EventTags__TagID__6C190EBB"),
                    l => l.HasOne<Event>().WithMany()
                        .HasForeignKey("EventId")
                        .HasConstraintName("FK__EventTags__Event__6B24EA82"),
                    j =>
                    {
                        j.HasKey("EventId", "TagId").HasName("PK__EventTag__AF1307D4112F0680");
                        j.ToTable("EventTags");
                        j.IndexerProperty<int>("EventId").HasColumnName("EventID");
                        j.IndexerProperty<int>("TagId").HasColumnName("TagID");
                    });
        });

        modelBuilder.Entity<EventCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__EventCat__19093A2B7259550D");

            entity.HasIndex(e => e.Name, "UQ__EventCat__737584F62409828E").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__6A4BEDF6435AC103");

            entity.HasIndex(e => new { e.EventId, e.UserId }, "UQ__Feedback__A83C44BBA11C66BA").IsUnique();

            entity.Property(e => e.FeedbackId).HasColumnName("FeedbackID");
            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.SubmittedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Event).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Feedbacks__Event__02084FDA");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Feedbacks__UserI__02FC7413");
        });

        modelBuilder.Entity<ForumCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__ForumCat__19093A2B2AD081F6");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<ForumComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__ForumCom__C3B4DFAA7937AB2C");

            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.CommentStatus)
                .HasMaxLength(30)
                .HasDefaultValue("Visible");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.ParentCommentId).HasColumnName("ParentCommentID");
            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.ParentComment).WithMany(p => p.InverseParentComment)
                .HasForeignKey(d => d.ParentCommentId)
                .HasConstraintName("FK__ForumComm__Paren__14270015");

            entity.HasOne(d => d.Post).WithMany(p => p.ForumComments)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__ForumComm__PostI__123EB7A3");

            entity.HasOne(d => d.User).WithMany(p => p.ForumComments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ForumComm__UserI__1332DBDC");
        });

        modelBuilder.Entity<ForumPost>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__ForumPos__AA1260387E8307A5");

            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.PostStatus)
                .HasMaxLength(30)
                .HasDefaultValue("Visible");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Category).WithMany(p => p.ForumPosts)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ForumPost__Categ__0C85DE4D");

            entity.HasOne(d => d.User).WithMany(p => p.ForumPosts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ForumPost__UserI__0B91BA14");
        });

        modelBuilder.Entity<Registration>(entity =>
        {
            entity.HasKey(e => e.RegistrationId).HasName("PK__Registra__6EF588309526F900");

            entity.HasIndex(e => e.UniqueToken, "UQ__Registra__64D6B76BE1D20530").IsUnique();

            entity.Property(e => e.RegistrationId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("RegistrationID");
            entity.Property(e => e.RegistrationDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.TicketTypeId).HasColumnName("TicketTypeID");
            entity.Property(e => e.UniqueToken).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.TicketType).WithMany(p => p.Registrations)
                .HasForeignKey(d => d.TicketTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Registrat__Ticke__75A278F5");

            entity.HasOne(d => d.User).WithMany(p => p.Registrations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Registrat__UserI__74AE54BC");
        });

        modelBuilder.Entity<SpeakerProfile>(entity =>
        {
            entity.HasKey(e => e.SpeakerId).HasName("PK__SpeakerP__79E7573992890D7B");

            entity.HasIndex(e => e.UserId, "UQ__SpeakerP__1788CCAD2DF218E4").IsUnique();

            entity.Property(e => e.SpeakerId).HasColumnName("SpeakerID");
            entity.Property(e => e.ApprovalStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.Company).HasMaxLength(255);
            entity.Property(e => e.JobTitle).HasMaxLength(255);
            entity.Property(e => e.LinkedInUrl)
                .HasMaxLength(500)
                .HasColumnName("LinkedInURL");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.WebsiteUrl)
                .HasMaxLength(500)
                .HasColumnName("WebsiteURL");

            entity.HasOne(d => d.User).WithOne(p => p.SpeakerProfile)
                .HasForeignKey<SpeakerProfile>(d => d.UserId)
                .HasConstraintName("FK__SpeakerPr__UserI__5629CD9C");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("PK__Tags__657CFA4C9C957E70");

            entity.HasIndex(e => e.TagName, "UQ__Tags__BDE0FD1D13352806").IsUnique();

            entity.Property(e => e.TagId).HasColumnName("TagID");
            entity.Property(e => e.TagName).HasMaxLength(100);
        });

        modelBuilder.Entity<TicketType>(entity =>
        {
            entity.HasKey(e => e.TicketTypeId).HasName("PK__TicketTy__6CD68451FC03ADAD");

            entity.Property(e => e.TicketTypeId).HasColumnName("TicketTypeID");
            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Event).WithMany(p => p.TicketTypes)
                .HasForeignKey(d => d.EventId)
                .HasConstraintName("FK__TicketTyp__Event__6EF57B66");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Transact__55433A4B252AAD91");

            entity.Property(e => e.TransactionId).HasColumnName("TransactionID");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GatewayTransactionId)
                .HasMaxLength(255)
                .HasColumnName("GatewayTransactionID");
            entity.Property(e => e.PaymentGateway)
                .HasMaxLength(50)
                .HasDefaultValue("PayOS");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.RegistrationId).HasColumnName("RegistrationID");
            entity.Property(e => e.TransactionDate).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Registration).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.RegistrationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacti__Regis__7C4F7684");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACA75F16C9");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105341FD32804").IsUnique();

            entity.Property(e => e.UserId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("UserID");
            entity.Property(e => e.AccountStatus)
                .HasMaxLength(20)
                .HasDefaultValue("PendingVerification");
            entity.Property(e => e.AddressCity)
                .HasMaxLength(100)
                .HasColumnName("Address_City");
            entity.Property(e => e.AddressDistrict)
                .HasMaxLength(100)
                .HasColumnName("Address_District");
            entity.Property(e => e.AddressStreet)
                .HasMaxLength(255)
                .HasColumnName("Address_Street");
            entity.Property(e => e.AddressWard)
                .HasMaxLength(100)
                .HasColumnName("Address_Ward");
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(500)
                .HasColumnName("AvatarURL");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.OauthProvider)
                .HasMaxLength(50)
                .HasColumnName("OAuthProvider");
            entity.Property(e => e.OauthProviderId)
                .HasMaxLength(255)
                .HasColumnName("OAuthProviderID");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValue("User");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasMany(d => d.Tags).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .HasConstraintName("FK__UserTags__TagID__208CD6FA"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__UserTags__UserID__1F98B2C1"),
                    j =>
                    {
                        j.HasKey("UserId", "TagId").HasName("PK__UserTags__C1DF030835A0D596");
                        j.ToTable("UserTags");
                        j.IndexerProperty<Guid>("UserId").HasColumnName("UserID");
                        j.IndexerProperty<int>("TagId").HasColumnName("TagID");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
