using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models.ViewModels
{
    public class CoachesEnterTeamsViewModel
    {
        public Tournament Tournament { get; set; }
        public List<Debater> Debaters { get; set; }
    }
}
