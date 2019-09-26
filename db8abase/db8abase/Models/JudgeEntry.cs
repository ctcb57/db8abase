using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models
{
    public class JudgeEntry
    {
        [Key]
        public int JudgeEntryId { get; set; }
        [ForeignKey("Tournament Id")]
        public int TournamentId { get; set; }
        [ForeignKey("Judge Id")]
        public int JudgeId { get; set; }
    }
}
