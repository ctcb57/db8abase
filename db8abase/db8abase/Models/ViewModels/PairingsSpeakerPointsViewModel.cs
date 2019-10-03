using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models.ViewModels
{
    public class PairingsSpeakerPointsViewModel
    {
        public List<Debater> SpeakerAwards { get; set; }
        public Debater Debater { get; set; }
        public List<int> Ranking { get; set; }
        public School School { get; set; }
        public List<School> Schools { get; set; }
        public Tournament Tournament { get; set; }
    }
}
