﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models
{
    public class Coach
    {
        [Key]
        public int CoachId { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Email { get; set; }
        [Display(Name = "Phone Number")]
        public int PhoneNumber { get; set; }
        public double Balance { get; set; }
        [ForeignKey("User Id")]
        public string ApplicationUserId { get; set; }
        [ForeignKey("School Id")]
        public int SchoolId { get; set; }
    }
}
