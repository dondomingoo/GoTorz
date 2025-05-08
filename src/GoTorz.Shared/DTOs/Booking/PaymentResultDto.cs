namespace GoTorz.Shared.DTOs.Booking
{
    public class PaymentResultDto
    {
        public string BookingId { get; set; }
        public string Status { get; set; } = "success"; // or "failed"
    }

}