namespace GoTorz.Shared.DTOs.Booking
{
    public class BookingResponseDto
    {
        public bool Success { get; set; }
        public string BookingId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string RedirectUrl { get; set; } = string.Empty;
    }

}