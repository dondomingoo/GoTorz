using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoTorz.Shared.DTOs
{
    
    
        public class OutboundFlightDto
        {
            public string FlightNumber { get; set; } = "";
            public string Departure { get; set; } = "";
            public string Destination { get; set; } = "";
            public DateTime DepartureTime { get; set; }
            public DateTime ArrivalTime { get; set; }
        }

        public class ReturnFlightDto : OutboundFlightDto { }
    

}
