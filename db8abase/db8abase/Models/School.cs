using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models
{
    public class School
    {
        [Key]
        public int SchoolId { get; set; }
        public string Name { get; set; }
        [ForeignKey("Address Id")]
        public int AddressId { get; set; }
        public Address Address { get; set; }

        [ForeignKey("Tournament Director Id")]
        public int TournamentDirectorId { get; set; }
        [ForeignKey("Coach Id")]
        public int CoachId { get; set; }
    }
}
