namespace GoTorz.Shared.DTOs.Booking
{
    public class BookingRequestDto
    {
        public string PackageId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PassportNumber { get; set; }
        public string PaymentMethod { get; set; }
        public string Currency { get; set; }
        public string UserId { get; set; }

        public List<TravellerDto> Travellers { get; set; } = new();

    }

}