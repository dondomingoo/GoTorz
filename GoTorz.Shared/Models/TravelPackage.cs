using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoTorz.Shared.Models
{
    public class TravelPackage
    {
        public string TravelPackageId { get; set; }
        public string Destination { get; set; }

        public DateTime Arrival { get; set; }
        public DateTime Departure { get; set; }

        public Hotel Hotel { get; set; }
        public OutboundFlight OutboundFlight { get; set; }
        public ReturnFlight ReturnFlight { get; set; }



        public string price { get; set; }
    }
}
