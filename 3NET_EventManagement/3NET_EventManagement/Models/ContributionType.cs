using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace _3NET_EventManagement.Models
{
    public class ContributionType
    {
        [HiddenInput(DisplayValue = false)]
        [Key]
        public int Id { get; set; }

        [Display(Name = "Type")]
        public string ContributionTypeName { get; set; }
    }
}
