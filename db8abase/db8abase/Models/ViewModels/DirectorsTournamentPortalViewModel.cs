using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models.ViewModels
{
    public class DirectorsTournamentPortalViewModel
    {
        public List<Round> Rounds { get; set; }
        public Tournament Tournament { get; set; }
        public Round Round { get; set; }
    }
}
