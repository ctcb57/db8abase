using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models
{
    public class IndividualTeam
    {
        [Key]
        public int IndividualTeamId { get; set; }
        public Debater FirstSpeaker { get; set; }
        public Debater SecondSpeaker { get; set; }
        public int SingleTournamentWins { get; set; }
        public int SingleTournamentLosses { get; set; }
        public int CumulativeAnnualWins { get; set; }
        public int CumulativeAnnualLosses { get; set; }
        public int CumulativeAnnualElminationRoundWins { get; set; }
        public int AnnualEliminationRoundAppearances { get; set; }
        public int TournamentAffirmativeRounds { get; set; }
        public int TournamentNegativeRounds { get; set; }
        [Display(Name = "Bids")]
        public int TocBids { get; set; }
        [ForeignKey("Coach Id")]
        public int CoachId { get; set; }
        [NotMapped]
        public List<IndividualTeam> TeamsFacedAtTournament { get; set; }
        [NotMapped]
        public List<Judge> JudgesSeenAtTournament { get; set; }

    }
}
