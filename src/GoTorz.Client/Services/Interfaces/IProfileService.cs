using GoTorz.Shared.DTOs.Booking;

namespace GoTorz.Client.Services.Interfaces
{
    public interface IProfileService
    {
        Task<(List<BookingDto> Upcoming, List<BookingDto> Past)> GetBookingsByUserAsync(string userId);
        Task<(bool Success, string Message)> DeleteUserAsync(string userId);
    }
}
