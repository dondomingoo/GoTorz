using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoTorz.Shared.Models
{
    public class Payment
    {
        public string PaymentId { get; set; }
        public string BookingId { get; set; }

        public string Amount { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
    }
}
