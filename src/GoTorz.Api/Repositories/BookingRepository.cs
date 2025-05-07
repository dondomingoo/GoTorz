using GoTorz.Api.Data;
using GoTorz.Shared.Models;
using Microsoft.EntityFrameworkCore;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Booking?> GetByIdAsync(string bookingId)
    {
        return await _context.Bookings
            .Include(b => b.TravelPackage)
            .Include(b => b.Travellers)
            .FirstOrDefaultAsync(b => b.Id == bookingId);
    }

    public async Task AddAsync(Booking booking)
    {
        await _context.Bookings.AddAsync(booking);
    }

    public async Task<IEnumerable<Booking>> GetFilteredAsync(
        string? userId, string? bookingId, DateTime? arrivalDate, DateTime? departureDate, DateTime? orderDate, string? email)
    {
        var query = _context.Bookings
            .Include(b => b.TravelPackage)
            .Include(b => b.Travellers)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(userId))
            query = query.Where(b => b.UserId == userId);
        if (!string.IsNullOrWhiteSpace(bookingId))
            query = query.Where(b => b.Id == bookingId);
        if (arrivalDate.HasValue)
            query = query.Where(b => b.TravelPackage.Arrival.Date == arrivalDate.Value.Date);
        if (departureDate.HasValue)
            query = query.Where(b => b.TravelPackage.Departure.Date == departureDate.Value.Date);
        if (orderDate.HasValue)
            query = query.Where(b => b.OrderDate.Date == orderDate.Value.Date);
        if (!string.IsNullOrWhiteSpace(email))
            query = query.Where(b => b.Email == email);

        return await query.ToListAsync();
    }

    public async Task DeleteAsync(string bookingId)
    {
        var booking = await _context.Bookings.FindAsync(bookingId);
        if (booking != null)
            _context.Bookings.Remove(booking);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
