using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoTorz.Shared.DTOs.Booking
{

    public class RetryPaymentDto
    {
        public string BookingId { get; set; }
        public string NewPaymentMethod { get; set; }
    }

}