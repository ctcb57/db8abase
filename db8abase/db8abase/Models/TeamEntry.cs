using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models
{
    public class TeamEntry
    {
        [Key]
        public int EntryId { get; set; }
        [ForeignKey("Tournament Id")]
        public int TournamentId { get; set; }
        [ForeignKey("Team Id")]
        public int IndividualTeamId { get; set; }
    }
}
