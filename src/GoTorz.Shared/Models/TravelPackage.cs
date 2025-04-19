using System;

namespace GoTorz.Shared.Models
{
    public class TravelPackage
    {
        public string TravelPackageId { get; set; } = Guid.NewGuid().ToString();
        public string Destination { get; set; } = "";

        public DateTime Arrival { get; set; }
        public DateTime Departure { get; set; }

        public decimal Price { get; set; }


        public int HotelId { get; set; }
        public Hotel Hotel { get; set; } = null!;

        public int OutboundFlightId { get; set; }
        public OutboundFlight OutboundFlight { get; set; } = null!;

        public int ReturnFlightId { get; set; }
        public ReturnFlight ReturnFlight { get; set; } = null!;
        public bool IsBooked { get; set; } = false;

        public string Currency { get; set; } = "dkk";
    }
}
