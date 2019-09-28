using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models
{
    public class TournamentResults
    {
        [Key]
        public int ResultsId { get; set; }
        [ForeignKey("Tournament Id")]
        public int TournamentId { get; set;  }
        [ForeignKey("Individual Team Id")]
        public int IndividualTeamId { get; set; }
        public int TeamWins { get; set; }
        public int TeamLosses { get; set; }
        public int SpeakerPoints { get; set; }
        public string EliminationRoundResult { get; set; }
    }
}
