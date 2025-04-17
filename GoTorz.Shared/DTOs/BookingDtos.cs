using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoTorz.Shared.DTOs
{
    public class BookingRequestDto
    {
        public string PackageId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PassportNumber { get; set; }
        public string PaymentMethod { get; set; }
        public string Currency { get; set; }

        public List<TravellerDto> Travellers { get; set; } = new();

    }

    public class TravellerDto
    {
        public string Name { get; set; }
        public string PassportNumber { get; set; }
    }

    public class BookingResponseDto
    {
        public bool Success { get; set; }
        public string BookingId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string RedirectUrl { get; set; } = string.Empty;
    }

    public class PaymentResultDto
    {
        public string BookingId { get; set; }
        public string Status { get; set; } = "success"; // or "failed"
    }

    public class PaymentResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string RedirectUrl { get; set; } = string.Empty;
    }

    public class RetryPaymentDto
    {
        public string BookingId { get; set; }
        public string NewPaymentMethod { get; set; }
    }

}
