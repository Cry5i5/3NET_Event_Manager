using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace _3NET_EventManagement.Models
{
    public class User
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserName { get; set; }

        public string Name { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }

        public int Age { get; set; }

        //public string Password { get; set; }
        public virtual ICollection<Event> EventsInvitedTo { get; set; }
        public virtual IList<User> Friends { get; set; }

        public virtual ICollection<Contribution> Contributions { get; set; }

        public User()
        {
            EventsInvitedTo = new HashSet<Event>();
            Contributions = new HashSet<Contribution>();
        }
    }
}