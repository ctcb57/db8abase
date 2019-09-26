using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models.ViewModels
{
    public class CoachesEnterJudgesViewModel
    {
        public IEnumerable<SelectListItem> Judges { get; set; }
        public JudgeEntry JudgeEntry { get; set; }

        public string Judge { get; set; }
    }
}
