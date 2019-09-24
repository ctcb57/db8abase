using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models
{
    public class Debater
    {
        [Key]
        public int DebaterId { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display (Name = "Last Name")]
        public string LastName { get; set; }
        public string Email { get; set; }
        [Display(Name = "Phone Number")]
        public int PhoneNumber {get; set;}
        [ForeignKey("Coach Id")]
        public int CoachId { get; set; }
        [ForeignKey("Debate Partner Id")]
        public int PartnerId { get; set; }
        [ForeignKey("School Id")]
        public int SchoolId { get; set; }
        public double? IndividualRoundSpeakerPoints { get; set; }
        public double IndividualTournamentSpeakerPoints { get; set; }
        public double AnnualAverageSpeakerPoints { get; set; }
        [ForeignKey("User Id")]
        public string ApplicationUserId { get; set; }
    }
}
