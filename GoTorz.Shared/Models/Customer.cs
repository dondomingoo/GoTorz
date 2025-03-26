using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoTorz.Shared.Models
{
    public class Customer : User
    {
        public ICollection<Booking>? Bookings { get; set; }
    }
}
