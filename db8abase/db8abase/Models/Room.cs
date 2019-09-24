using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models
{
    public class Room
    {
        [Key]
        public int RoomId { get; set; }
        public int RoomNumber { get; set; }
        [ForeignKey("School Id")]
        public int SchoolId { get; set; }
    }
}
