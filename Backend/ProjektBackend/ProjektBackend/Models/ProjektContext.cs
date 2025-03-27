using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProjektBackend.Models;

public partial class ProjektContext : DbContext
{
    public ProjektContext()
    {
    }

    public ProjektContext(DbContextOptions<ProjektContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Connection> Connections { get; set; }

    public virtual DbSet<Employer> Employers { get; set; }

    public virtual DbSet<Employerrequest> Employerrequests { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Profile> Profiles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Connection>(entity =>
        {
            entity.HasKey(e => e.ConnectionId).HasName("PRIMARY");

            entity.ToTable("connections");

            entity.HasIndex(e => e.ReceiverId, "ReceiverID");

            entity.HasIndex(e => new { e.RequesterId, e.ReceiverId }, "idx_unique_connection").IsUnique();

            entity.Property(e => e.ConnectionId)
                .HasColumnType("int(11)")
                .HasColumnName("ConnectionID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");
            entity.Property(e => e.ReceiverId)
                .HasColumnType("int(11)")
                .HasColumnName("ReceiverID");
            entity.Property(e => e.RequesterId)
                .HasColumnType("int(11)")
                .HasColumnName("RequesterID");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'''Pending'''")
                .HasColumnType("enum('Pending','Accepted','Declined')");

            entity.HasOne(d => d.Receiver).WithMany(p => p.ConnectionReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .HasConstraintName("connections_ibfk_2");

            entity.HasOne(d => d.Requester).WithMany(p => p.ConnectionRequesters)
                .HasForeignKey(d => d.RequesterId)
                .HasConstraintName("connections_ibfk_1");
        });

        modelBuilder.Entity<Employer>(entity =>
        {
            entity.HasKey(e => e.EmployerId).HasName("PRIMARY");

            entity.ToTable("employers");

            entity.HasIndex(e => e.UserId, "UserID");

            entity.HasIndex(e => e.UserId, "UserID_2").IsUnique();

            entity.Property(e => e.EmployerId)
                .HasColumnType("int(11)")
                .HasColumnName("EmployerID");
            entity.Property(e => e.CompanyAddress)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.CompanyDescription)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text");
            entity.Property(e => e.CompanyEmail).HasMaxLength(255);
            entity.Property(e => e.CompanyName).HasMaxLength(255);
            entity.Property(e => e.CompanyPhoneNumber).HasColumnType("int(11)");
            entity.Property(e => e.CompanyWebsite)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.EstablishedYear)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("year(4)");
            entity.Property(e => e.Industry)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("UserID");

            entity.HasOne(d => d.User).WithOne(p => p.Employer)
                .HasForeignKey<Employer>(d => d.UserId)
                .HasConstraintName("employers_ibfk_1");
        });

        modelBuilder.Entity<Employerrequest>(entity =>
        {
            entity.HasKey(e => e.ApplicantId).HasName("PRIMARY");

            entity.ToTable("employerrequest");

            entity.HasIndex(e => e.UserId, "UserID");

            entity.HasIndex(e => e.UserId, "UserID_2").IsUnique();

            entity.Property(e => e.ApplicantId)
                .HasColumnType("int(11)")
                .HasColumnName("ApplicantID");
            entity.Property(e => e.CompanyAddress).HasMaxLength(255);
            entity.Property(e => e.CompanyDescription).HasColumnType("text");
            entity.Property(e => e.CompanyEmail).HasMaxLength(255);
            entity.Property(e => e.CompanyName).HasMaxLength(255);
            entity.Property(e => e.CompanyPhoneNumber).HasColumnType("int(11)");
            entity.Property(e => e.CompanyWebsite).HasMaxLength(255);
            entity.Property(e => e.EstabilishedYear).HasColumnType("year(4)");
            entity.Property(e => e.Industry).HasMaxLength(255);
            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("UserID");

            entity.HasOne(d => d.User).WithOne(p => p.Employerrequest)
                .HasForeignKey<Employerrequest>(d => d.UserId)
                .HasConstraintName("employerrequest_ibfk_1");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PRIMARY");

            entity.ToTable("messages");

            entity.HasIndex(e => e.ReceiverId, "ReceiverID");

            entity.HasIndex(e => e.SenderId, "SenderID");

            entity.Property(e => e.MessageId)
                .HasColumnType("int(11)")
                .HasColumnName("MessageID");
            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.ReceiverId)
                .HasColumnType("int(11)")
                .HasColumnName("ReceiverID");
            entity.Property(e => e.SenderId)
                .HasColumnType("int(11)")
                .HasColumnName("SenderID");
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Receiver).WithMany(p => p.MessageReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .HasConstraintName("messages_ibfk_2");

            entity.HasOne(d => d.Sender).WithMany(p => p.MessageSenders)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("messages_ibfk_1");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PRIMARY");

            entity.ToTable("posts");

            entity.HasIndex(e => e.EmployerId, "EmployerID");

            entity.Property(e => e.PostId)
                .HasColumnType("int(11)")
                .HasColumnName("PostID");
            entity.Property(e => e.Category).HasMaxLength(255);
            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployerId)
                .HasColumnType("int(11)")
                .HasColumnName("EmployerID");
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.Title).HasColumnType("text");
            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("UserID");

            entity.HasOne(d => d.Employer).WithMany(p => p.Posts)
                .HasForeignKey(d => d.EmployerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Posts_Employers");
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("PRIMARY");

            entity.ToTable("profiles");

            entity.HasIndex(e => e.UserId, "UserID");

            entity.Property(e => e.ProfileId)
                .HasColumnType("int(11)")
                .HasColumnName("ProfileID");
            entity.Property(e => e.Bio)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text");
            entity.Property(e => e.Banner)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.ProfilePicture)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Profiles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("profiles_ibfk_1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "Email").IsUnique();

            entity.HasIndex(e => e.Email, "Email_2").IsUnique();

            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");
            entity.Property(e => e.FirstName).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValueSql("'1'");
            entity.Property(e => e.LastName).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.RefreshToken).HasMaxLength(255);
            entity.Property(e => e.Role)
                .HasDefaultValueSql("'''Employee'''")
                .HasColumnType("enum('Employee','Employer','Admin')");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
