namespace _3NET_EventManagement.Migrations
{
    using _3NET_EventManagement.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<_3NET_EventManagement.Models.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            
           // ContextKey = "_3NET_EventManagement.Models.AppDbContext";
        }

        protected override void Seed(_3NET_EventManagement.Models.AppDbContext context)
        {

          /*  var statuses = new List<Status>
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

            context.SaveChanges();*/
        }
    }
}
