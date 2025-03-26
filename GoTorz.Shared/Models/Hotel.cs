using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoTorz.Shared.Models
{
    public class Hotel
    {
        public int HotelId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime Checkin { get; set; }
        public DateTime Checkout { get; set; }
        public int Rooms { get; set; }
    }
}
