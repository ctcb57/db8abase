using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models
{
    public class Tournament
    {
        [Key]
        public int TournamentId { get; set; }
        public string Name { get; set; }
        [ForeignKey("School Id")]
        public int SchoolId { get; set; }
        public School School { get; set; }
        [Display(Name = "Number of Rounds")]
        public int NumberOfRounds { get; set; }
        public int NumberOfEliminationRounds { get; set; }
        public double EntryFee { get; set; }
        [Display(Name = "Tournament Date")]
        [DataType(DataType.Date)]
        public DateTime TournamentDate { get; set; }
        [Display(Name = "Team Limit")]
        public int TeamLimit { get; set; }
        public string FilePath { get; set; }
    }
}
