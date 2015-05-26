using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _3NET_EventManagement.Models
{
    public class AddFriendViewModel
    {
        [Display(Name = "Username or Name")]
        public string FriendName { get; set; }

    }
}