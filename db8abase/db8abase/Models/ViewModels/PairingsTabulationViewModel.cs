using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models.ViewModels
{
    public class PairingsTabulationViewModel
    {
        public Tournament Tournament { get; set; }
        public List<Judge> Judges { get; set; }
        public Judge Judge { get; set; }
        public List<IndividualTeam> AffirmativeTeams { get; set; }
        public List<IndividualTeam> NegativeTeams { get; set; }
        public IndividualTeam IndividualTeam { get; set; }
        public List<Room> Rooms { get; set; }
        public Room Room { get; set; }
        public Round Round { get; set; }
    }
}
