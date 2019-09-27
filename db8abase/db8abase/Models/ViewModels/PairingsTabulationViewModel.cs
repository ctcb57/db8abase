using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models.ViewModels
{
    public class PairingsTabulationViewModel
    {
        public Tournament Tournament { get; set; }
        public List<Judge> Judge { get; set; }
    }
}
