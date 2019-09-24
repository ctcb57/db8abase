using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models
{
    public class Round
    {
        [Key]
        public int RoundId { get; set; }
        [Display(Name = "Round Number")]
        public int RoundNumber { get; set; }
        [Display(Name = "Round Type")]
        public string RoundType { get; set; }
        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        public DateTime StartTime { get; set; }
        [NotMapped]
        public List<Debate> Debates { get; set; }
    }
}
