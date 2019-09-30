using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models
{
    public class Ballot
    {
        [Key]
        public int BallotId { get; set; }
        [ForeignKey("Judge Id")]
        public int JudgeId { get; set; }
        [ForeignKey("Round Id")]
        public int RoundId { get; set; }
        [ForeignKey("Tournament Id")]
        public int TournamentId { get; set; }
        [ForeignKey("Debate Id")]
        public int DebateId { get; set; }
        public double FirstAffSpeakerPoints { get; set; }
        public double SecondAffSpeakerPoints { get; set; }
        public double FirstNegSpeakerPoints { get; set; }
        public double SecondNegSpeakerPoints { get; set; }
        [ForeignKey("Winner Id")]
        public int WinnerId { get; set; }
        public string ReasonForDecision { get; set; }
        public bool BallotTurnedIn { get; set; }
    }
}
