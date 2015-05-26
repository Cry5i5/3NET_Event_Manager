using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _3NET_EventManagement.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("DefaultConnection")
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<ContributionType> ContributionTypes { get; set; }
        public DbSet<Contribution> Contributions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasMany(u => u.Friends).WithMany();
            modelBuilder.Entity<Event>()
                .HasMany<User>(e => e.InvitedUsers)
                .WithMany(u => u.EventsInvitedTo)
                .Map(c =>
                {
                    c.MapLeftKey("Event_id");
                    c.MapRightKey("User_id");
                    c.ToTable("Invitations");
                });
        }
    }

    public class ApplicationDbContextInitializer : DropCreateDatabaseIfModelChanges<AppDbContext>
    {
        protected override void Seed(AppDbContext context)
        {
            var statuses = new List<Status>
            {
                new Status {Id = 1, StatusName = "Open"},
                new Status {Id = 2, StatusName = "Pending"},
                new Status {Id = 3, StatusName = "Closed"}
            };

            var eventTypes = new List<EventType>
            {
                new EventType {Id = 1, TypeName = "Party"},
                new EventType {Id = 2, TypeName = "Lunch"},
                new EventType {Id = 3, TypeName = "Break"},
                new EventType {Id = 3, TypeName = "Beach Party"}
            };

            var contributionTypes = new List<ContributionType>
            {
                new ContributionType {Id = 1, ContributionTypeName = "Food"},
                new ContributionType {Id = 2, ContributionTypeName = "Money"},
                new ContributionType {Id = 3, ContributionTypeName = "Soft drink"},
                new ContributionType {Id = 4, ContributionTypeName = "Hard drink"}
            };

            foreach (ContributionType r in contributionTypes)
            {
                context.ContributionTypes.Add(r);
            }

            foreach (Status r in statuses)
            {
                context.Statuses.Add(r);
            }

            foreach (EventType r in eventTypes)
            {
                context.EventTypes.Add(r);
            }

            context.SaveChanges();
        }
    }

}