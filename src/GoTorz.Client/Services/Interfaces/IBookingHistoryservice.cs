using GoTorz.Shared.DTOs;
using GoTorz.Shared.Models;

namespace GoTorz.Client.Services.Interfaces
{
    public interface IBookingHistoryservice
    {
        Task<List<BookingDto>> GetBookingHistoryAsync(string? userId, string? bookingID, DateTime? arrivalDate, DateTime? departureDate, DateTime? orderDate, string? email);
        Task<bool> CancelBookingAsync(string bookingId);
    }
}