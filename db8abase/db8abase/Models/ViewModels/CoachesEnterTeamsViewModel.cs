using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models.ViewModels
{
    public class CoachesEnterTeamsViewModel
    {
        public Tournament Tournament { get; set; }
        public List<SelectListItem> Debaters { get; set; }
    }
}
