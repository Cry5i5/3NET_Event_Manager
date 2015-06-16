using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace _3NET_EventManagement.Models
{
   public class Contribution
    {
        [HiddenInput(DisplayValue = false)]
        [Key]
        public int Id { get; set; }

        public int Amount { get; set; }

       [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public int EventId { get; set; }
        public virtual Event Event { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int TypeId { get; set; }
        public virtual ContributionType Type { get; set; }

        public Contribution()
        {

        }
    }
}
