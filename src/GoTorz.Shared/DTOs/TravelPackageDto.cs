using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoTorz.Shared.DTOs
{
    public class TravelPackageDto
    {
        public string Destination { get; set; } = "";
        public DateTime Arrival { get; set; }
        public DateTime Departure { get; set; }
        public HotelDto Hotel { get; set; } = new();
        public OutboundFlightDto OutboundFlight { get; set; } = new();
        public ReturnFlightDto ReturnFlight { get; set; } = new();
        public string Price { get; set; } = "";
    }
}
