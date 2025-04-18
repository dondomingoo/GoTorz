using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoTorz.Shared.DTOs
{
    public class FlightSearchResultDto
    {
        public OutboundFlightDto OutboundFlight { get; set; }
        public ReturnFlightDto ReturnFlight { get; set; }
        public string TotalPrice { get; set; }
    }
}
