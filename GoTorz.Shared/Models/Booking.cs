using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoTorz.Shared.Models
{
    public class Booking
    {
        public string BookingId { get; set; }
        public string UserId { get; set; }
        public List<Traveller> Travellers { get; set; } = [];
        public string TravelPackageId { get; set; }
        public TravelPackage TravelPackage { get; set; } = new();
        public string PaymentStatus { get; set; }

        public DateTime OrderDate { get; set; }

    }
}
