using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models.ViewModels
{
    public class HomeDetailsViewModel
    {
        public Tournament Tournament { get; set; }

        public List<IndividualTeam> Teams { get; set; }

        public List<Judge> Judges { get; set; }
    }
}
