using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoTorz.Shared.Models
{
    public class Traveller
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PassportNumber { get; set; }

        public string BookingId { get; set; }     // FK
        public Booking Booking { get; set; }      // Navigation
    }
}
