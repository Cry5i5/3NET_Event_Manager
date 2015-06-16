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
            Configuration.ProxyCreationEnabled = true;
            Configuration.LazyLoadingEnabled = true;
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<ContributionType> ContributionTypes { get; set; }
        public DbSet<Contribution> Contributions { get; set; }
        public DbSet<Invitation> Invitations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // on set les contraintes pour éviter les soucis en cas de suppression en cascade
            modelBuilder.Entity<User>().HasMany(u => u.Friends).WithMany();
            modelBuilder.Entity<Contribution>().HasRequired(c => c.User).WithMany(c => c.Contributions).HasForeignKey(c => c.UserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Invitation>().HasRequired(c => c.Event).WithMany(c => c.Invitations).HasForeignKey(c => c.EventId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Invitation>().HasRequired(c => c.User).WithMany(c => c.EventsInvitedTo).HasForeignKey(c => c.UserId).WillCascadeOnDelete(false);
          
           
        }
       
    }

    public class ApplicationDbContextInitializer : DropCreateDatabaseIfModelChanges<AppDbContext>
    {

        public ApplicationDbContextInitializer()
        {
            using (var db = new AppDbContext())
            {
                Seed(db);
            }
        }
       
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
            if (context.ContributionTypes.Count() == 0 && context.Statuses.Count() == 0 && context.EventTypes.Count() == 0)
            {


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

}