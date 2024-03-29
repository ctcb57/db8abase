﻿using System;
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
        public List<IndividualTeam> Winners { get; set; }

        public List<Judge> Judges { get; set; }
        public List<Round> Rounds { get; set; }
        public IndividualTeam Team { get; set; }

        public TournamentResults Result { get; set; }
    }
}
