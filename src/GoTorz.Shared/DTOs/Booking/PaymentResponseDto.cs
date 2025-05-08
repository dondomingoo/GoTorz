namespace GoTorz.Shared.DTOs.Booking
{
    public class PaymentResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string RedirectUrl { get; set; } = string.Empty;

        public string? Destination { get; set; }
        public DateTime? Arrival { get; set; }
        public DateTime? Departure { get; set; }
    }

}