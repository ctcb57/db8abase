using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models
{
    public class Tournament
    {
        [Key]
        public int TournamentId { get; set; }
        public string Name { get; set; }
        public School School { get; set; }
        [Display(Name = "Number of Rounds")]
        public int NumberOfRounds { get; set; }

    }
}
