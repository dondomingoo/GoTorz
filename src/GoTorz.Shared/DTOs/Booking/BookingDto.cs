namespace GoTorz.Shared.DTOs.Booking
{
    public class BookingDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public DateTime Arrival { get; set; }
        public DateTime Departure { get; set; }

        public List<TravellerDto> Travellers { get; set; } = new();
    }
}
