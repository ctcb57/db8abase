﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models.ViewModels
{
    public class CoachesViewTournamentDetailsViewModel
    {
        public List<IndividualTeam> Teams { get; set; }
        public Coach Coach { get; set; }
        public Tournament Tournament { get; set; }
    }
}
