using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoTorz.Shared.Models
{
    public class ReturnFlight : Flight
    {
        public ICollection<TravelPackage> TravelPackages { get; set; } = new List<TravelPackage>();
    }
}
