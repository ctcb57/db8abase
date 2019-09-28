using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models
{
    public class Pairing
    {
        [Key]
        public int PairingId { get; set; }
        [ForeignKey("Tournament Id")]
        public int TournamentId { get; set; }
        [ForeignKey("Judge Id")]
        public int JudgeId { get; set; }
        [ForeignKey("Room Id")]
        public int RoomId { get; set; }
        [ForeignKey("Affirmative Team")]
        public int AffirmativeTeamId { get; set; }
        [ForeignKey("Negative Team")]
        public int NegativeTeamId { get; set; }
        [ForeignKey("Round Id")]
        public int RoundId { get; set; }
        [ForeignKey("Debate Id")]
        public int DebateId { get; set; }
        [ForeignKey("Winner Id")]
        public int WinnerId { get; set; }



    }
}
