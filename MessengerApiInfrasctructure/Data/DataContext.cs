using BCrypt.Net;
using MessengerApiDomain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerApiInfrasctructure.Data;

public class DataContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<CalendarEvent> CalendarEvents { get; set; }
    public DbSet<UserEventAttendance> EventAttendance { get; set; }
    public DbSet<OperationLog> OperationLogs { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany(user => user.Chats)
            .WithMany(chat => chat.Participants)
            .UsingEntity("ChatParticipants");

        modelBuilder.Entity<User>()
            .HasMany(u => u.Blacklist)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "Blacklist",
                j => j
                    .HasOne<User>()
                    .WithMany()
                    .HasForeignKey("BlacklistedUserId")
                    .OnDelete(DeleteBehavior.Restrict),
                j => j
                    .HasOne<User>()
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Restrict));

        modelBuilder.Entity<Message>()
            .HasOne(message => message.Author)
            .WithMany(user => user.Messages)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
            .HasMany(message => message.UserReports)
            .WithMany(user => user.ReportedMessages)
            .UsingEntity("ReportedMessages");

        modelBuilder.Entity<Message>()
            .HasMany(message => message.Likes)
            .WithMany(user => user.LikedMessages)
            .UsingEntity("LikedMessages");

        modelBuilder.Entity<CalendarEvent>()
            .HasOne(calendarEvent => calendarEvent.Organizer)
            .WithMany(user => user.OrganizedEvents)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CalendarEvent>()
            .HasMany(calendarEvent => calendarEvent.Participants)
            .WithMany(user => user.Events)
            .UsingEntity("UserCalendarEvents");

        modelBuilder.Entity<UserEventAttendance>()
            .HasOne(eventParticipation => eventParticipation.Event)
            .WithMany(calendarEvent => calendarEvent.Attendance);

        var moderator = new User
        {
            Id = Guid.NewGuid(),
            Name = "Admin",
            DisplayName = "Admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin"),
            AvatarUrl = "http://127.0.0.1:10000/devstoreaccount1/messenger-container/default-avatar.png",
            IsAdmin = true,
        };

        var eventBot = new User
        {
            Id = Guid.NewGuid(),
            Name = "EventBot",
            DisplayName = "Event bot",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("EventBot"),
            AvatarUrl = "http://127.0.0.1:10000/devstoreaccount1/messenger-container/event-bot.png",
            IsAdmin = false,
        };

        modelBuilder.Entity<User>().HasData(moderator);
        modelBuilder.Entity<User>().HasData(eventBot);
    }
}
