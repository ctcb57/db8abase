﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models.ViewModels
{
    public class CoachesViewReportsViewModel
    {
        public List<SimpleReportViewModel> FirstTeamReports { get; set; }
        public List<SimpleReportViewModel> SecondTeamReports { get; set; }
        public List<IndividualTeam> Teams { get; set; }

        public string TournamentName { get; set; }
        public int Wins { get; set; }
        public int SpeakerPoints { get; set; }
        public List<SelectListItem> FirstTeamList { get; set; }
        public List<SelectListItem> SecondTeamList { get; set; }
        public List<SelectListItem> Graphs { get; set; }
        public List<TournamentResults> Results { get; set; }

        public IndividualTeam TeamOne { get; set; }
        public IndividualTeam TeamTwo { get; set; }
        public string TeamA { get; set; }
        public string TeamB { get; set; }
        public string GraphNumber { get; set; }

    }
}
