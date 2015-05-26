using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _3NET_EventManagement.Models
{
    public class InviteFriendModel
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Friend")]
        public string FriendUserId { get; set; }

        public virtual User Friend { get; set; }
    }
}