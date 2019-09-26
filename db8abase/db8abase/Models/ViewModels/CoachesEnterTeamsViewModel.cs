using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models.ViewModels
{
    public class CoachesEnterTeamsViewModel
    {
        public IEnumerable<SelectListItem> Teams { get; set; }
        public TeamEntry TeamEntry { get; set; }

        public string Team { get; set; }

    }
}
