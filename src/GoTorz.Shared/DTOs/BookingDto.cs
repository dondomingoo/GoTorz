namespace GoTorz.Shared.DTOs
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
    }
}
