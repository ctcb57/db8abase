using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }
        [ForeignKey("School Id")]
        public int SchoolId { get; set; }
        [ForeignKey("Coach Id")]
        public int CoachId { get; set; }
        [NotMapped]
        public List<IndividualTeam> TeamList { get; set; }
    }
}
