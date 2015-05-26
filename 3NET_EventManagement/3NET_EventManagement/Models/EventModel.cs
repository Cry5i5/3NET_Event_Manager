using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _3NET_EventManagement.Models
{
    public class EventModel
    {
        public EventModel(int id, string EventName, string address )
        {
            InvitedUsers = new HashSet<User>();
            this.Id = Id;
        }

        [HiddenInput(DisplayValue = false)]
       
        public int Id { get; set; }

        [Display(Name = "Event name")]
        public string EventName { get; set; }

        [Display(Name = "Event location")]
        public string Address { get; set; }

        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Display(Name = "Summary")]
        [DataType(DataType.MultilineText)]
        public string Summary { get; set; }

        // relation one to many
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }

        // de meme pour le status , un évenement a un status et un type
        [Display(Name = "Status")]
        public int StatusId { get; set; }

        public virtual Status Status { get; set; }

        [Display(Name = "Type")]
        public int TypeId { get; set; }

        public virtual EventType Type { get; set; }

        // Un évenement possède plusieurs participants et contributions
        [Display(Name = "Invited users")]
        public virtual ICollection<User> InvitedUsers { get; set; }

        public virtual ICollection<Contribution> Contributions { get; set; }
    }
}