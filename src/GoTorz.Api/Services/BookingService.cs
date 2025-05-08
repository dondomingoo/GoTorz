
using GoTorz.Api.Adapters;
using GoTorz.Api.Data;
using GoTorz.Api.Services;
using GoTorz.Shared.DTOs.Booking;
using GoTorz.Shared.Models;
using Microsoft.EntityFrameworkCore;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ITravelPackageRepository _packageRepository;
    private readonly IPaymentAdapter _paymentAdapter;

    public BookingService(
        IBookingRepository bookingRepository,
        ITravelPackageRepository packageRepository,
        IPaymentAdapter paymentAdapter)
    {
        _bookingRepository = bookingRepository;
        _packageRepository = packageRepository;
        _paymentAdapter = paymentAdapter;
    }

    public async Task<BookingResponseDto> SubmitCustomerInfoAsync(BookingRequestDto request)
    {
        try
        {
            var package = await _packageRepository.GetByIdAsync(request.PackageId);
            if (package == null)
                return new BookingResponseDto { Success = false, Message = "Package not found." };

            if (package.IsBooked)
                return new BookingResponseDto { Success = false, Message = "Package already booked." };

            var booking = new Booking
            {
                Id = Guid.NewGuid().ToString(),
                TravelPackageId = request.PackageId,
                FullName = request.FullName,
                Email = request.Email,
                PassportNumber = request.PassportNumber,
                PaymentStatus = "Pending",
                UserId = request.UserId,
                Travellers = request.Travellers.Select(t => new Traveller
                {
                    Name = t.Name,
                    PassportNumber = t.PassportNumber
                }).ToList()
            };

            await _bookingRepository.AddAsync(booking);
            await _bookingRepository.SaveChangesAsync();

            var totalPrice = package.Price;
            var currency = string.IsNullOrWhiteSpace(request.Currency)
                ? (package.Currency?.ToLower() ?? "eur")
                : request.Currency.ToLower();

            var redirectUrl = await _paymentAdapter.CreateCheckoutSessionAsync(
                booking.Id, totalPrice, currency,
                $"Trip to {package.Destination}",
                $"Hotel: {package.Hotel.Name}, Dates: {package.Arrival:yyyy-MM-dd} to {package.Departure:yyyy-MM-dd}");

            return new BookingResponseDto
            {
                Success = true,
                BookingId = booking.Id,
                RedirectUrl = redirectUrl
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Booking][ERROR] {ex.Message}");
            return new BookingResponseDto
            {
                Success = false,
                Message = "An unexpected error occurred during booking. Please try again later."
            };
        }
    }

    public async Task<TravelPackage?> InitiateBookingAsync(string packageId)
    {
        var package = await _packageRepository.GetByIdAsync(packageId);
        return package?.IsBooked == false ? package : null;
    }

    public Task<PaymentResponseDto> RetryPaymentAsync(RetryPaymentDto retry)
    {
        return Task.FromResult(new PaymentResponseDto
        {
            Success = false,
            Message = "Retry payment not implemented. Please start a new booking."
        });
    }

    public async Task<PaymentResponseDto> ConfirmPaymentAsync(PaymentResultDto payment)
    {
        var booking = await _bookingRepository.GetByIdAsync(payment.BookingId);
        if (booking == null)
            return new PaymentResponseDto { Success = false, Message = "Booking not found." };

        if (payment.Status == "success")
        {
            booking.PaymentStatus = "Confirmed";

            var package = await _packageRepository.GetByIdAsync(booking.TravelPackageId);
            if (package != null)
            {
                package.IsBooked = true;
                await _packageRepository.SaveChangesAsync();
            }

            await _bookingRepository.SaveChangesAsync();

            return new PaymentResponseDto
            {
                Success = true,
                Message = $"Thank you! Your trip to {package?.Destination} from {package?.Arrival:MMMM d, yyyy} to {package?.Departure:MMMM d, yyyy} has been successfully booked.",
                Destination = package?.Destination,
                Arrival = package?.Arrival,
                Departure = package?.Departure
            };
        }

        booking.PaymentStatus = "PaymentFailed";
        await _bookingRepository.SaveChangesAsync();

        return new PaymentResponseDto { Success = false, Message = "Payment failed." };
    }

    public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync(
        string? userId = null,
        string? bookingId = null,
        DateTime? arrivalDate = null,
        DateTime? departureDate = null,
        DateTime? orderDate = null,
        string? email = null)
    {
        var bookings = await _bookingRepository.GetFilteredAsync(
            userId, bookingId, arrivalDate, departureDate, orderDate, email);

        return bookings.Select(b => new BookingDto
        {
            Id = b.Id,
            Email = b.Email,
            CustomerName = b.FullName,
            OrderDate = b.OrderDate,
            Status = b.PaymentStatus,
            Arrival = b.TravelPackage.Arrival,
            Departure = b.TravelPackage.Departure,
            Travellers = new List<TravellerDto>
    {
        new TravellerDto
        {
            Name = b.FullName,
            PassportNumber = b.PassportNumber
        }
    }
    .Concat(
        b.Travellers.Select(t => new TravellerDto
        {
            Name = t.Name,
            PassportNumber = t.PassportNumber
        })
    ).ToList()
        });
    }

    public async Task<bool> CancelBookingAsync(string bookingId)
    {
        await _bookingRepository.DeleteAsync(bookingId);
        await _bookingRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> HasUpcomingBookingsAsync(string userId)
    {
        return await _bookingRepository.HasUpcomingBookingsAsync(userId);
    }


}


