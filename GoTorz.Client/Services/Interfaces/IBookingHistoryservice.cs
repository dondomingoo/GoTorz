using GoTorz.Shared.Models;

namespace GoTorz.Client.Services.Interfaces
{
    public interface IBookingHistoryservice
    {
        Task<List<Booking>> GetBookingHistoryAsync(string? userId, string? bookingID, DateTime? arrivalDate, DateTime? departureDate, DateTime? orderDate);
        Task<bool> CancelBookingAsync(string bookingId);
    }
}
