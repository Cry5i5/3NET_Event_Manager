using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _3NET_EventManagement.Models
{
    public class Invitation
    {
        [Key]
        public int Id { get; set; }
        
        public int EventId { get; set; }
        public Event Event { get; set; }
        public int UserId { get; set; }

        public User User { get; set; }

        
        
    }
}