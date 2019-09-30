using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models.ViewModels
{
    public class JudgesBallotViewModel
    {
        public Ballot Ballot { get; set; }
        public string Winner { get; set; }
        public string Loser { get; set; }

        public IEnumerable<SelectListItem> TeamsInRound { get; set; }

        public IndividualTeam AffirmativeTeam { get; set; }
        public IndividualTeam NegativeTeam { get; set; }
        public Round Round { get; set;}
        public Debate Debate { get; set; }
        public Judge Judge { get; set; }


    }
}
