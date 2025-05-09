using GoTorz.Shared.Models;

/// <summary>
/// Repository interface for managing bookings.
/// </summary>
public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(string bookingId);
    Task AddAsync(Booking booking);
    Task<IEnumerable<Booking>> GetFilteredAsync(
        string? userId = null,
        string? bookingId = null,
        DateTime? arrivalDate = null,
        DateTime? departureDate = null,
        DateTime? orderDate = null,
        string? email = null);
    Task DeleteAsync(string bookingId);
    Task SaveChangesAsync();
    Task<bool> HasUpcomingBookingsAsync(string userId);
}
