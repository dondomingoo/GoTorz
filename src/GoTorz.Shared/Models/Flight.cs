using System;
using System.ComponentModel.DataAnnotations;

namespace GoTorz.Shared.Models
{
    public abstract class Flight
    {
        [Key]
        public int FlightId { get; set; }

        public string FlightNumber { get; set; } = "";
        public string Departure { get; set; } = "";
        public string Destination { get; set; } = "";
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
    }
}
