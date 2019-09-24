using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models
{
    public class Debate
    {
        [Key]
        public int DebateId { get; set; }
        [ForeignKey("Room Id")]
        public int RoomId { get; set; }
        [ForeignKey("Judge Id")]
        public int JudgeId { get; set; }
        [ForeignKey("Affirmative Team Id")]
        public int AffirmativeTeamId { get; set; }
        [ForeignKey("Negative Team Id")]
        public int NegativeTeamId { get; set; }
        public bool AffirmativeWinsRound { get; set; }
    }
}
