using MessengerApiDomain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerApiInfrasctructure.Data
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(user => user.Chats)
                .WithMany(chat => chat.Participants);

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
                .WithMany(user => user.ReportedMessages);
        }
    }
}
