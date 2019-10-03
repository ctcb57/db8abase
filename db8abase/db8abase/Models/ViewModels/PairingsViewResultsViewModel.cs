using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models.ViewModels
{
    public class PairingsViewResultsViewModel
    {
        public List<int> Ranking { get; set; }
        public List<IndividualTeam> Teams { get; set; }
        public Tournament Tournament { get; set; }

        public List<TournamentResults> Results { get; set; }

    }
}
