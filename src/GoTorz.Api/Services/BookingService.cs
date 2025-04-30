//using GoTorz.Api.Data;
//using GoTorz.Api.Migrations;
//using GoTorz.Shared.DTOs;
//using GoTorz.Shared.Models;
//using Microsoft.Extensions.Configuration;
//using Stripe.Checkout;

//namespace GoTorz.Api.Services
//{
//    public class BookingService : IBookingService
//    {
//        private readonly ApplicationDbContext _db;
//        private readonly IConfiguration _config;

//        public BookingService(ApplicationDbContext db, IConfiguration config)
//        {
//            _db = db;
//            _config = config;
//        }

//        public async Task<TravelPackage?> InitiateBookingAsync(string packageId)
//        {
//            var package = await _db.TravelPackages.FindAsync(packageId);
//            return package?.IsBooked == false ? package : null;
//        }

//        public async Task<BookingResponseDto> SubmitCustomerInfoAsync(BookingRequestDto request)
//        {
//            var package = await _db.TravelPackages.FindAsync(request.PackageId);
//            if (package == null || package.IsBooked)
//                return new BookingResponseDto { Success = false, Message = "Package not available." };

//            var booking = new Booking
//            {
//                Id = Guid.NewGuid().ToString(),
//                TravelPackageId = request.PackageId,
//                FullName = request.FullName,
//                Email = request.Email,
//                PassportNumber = request.PassportNumber,
//                PaymentStatus = "Pending"
//            };

//            await _db.Bookings.AddAsync(booking);
//            await _db.SaveChangesAsync();

//            //int totalPeople = 1 + (request.Travellers?.Count ?? 0);
//            decimal totalPrice = package.Price;
//            var currency = string.IsNullOrWhiteSpace(request.Currency)
//            ? (package.Currency?.ToLower() ?? "eur")
//            : request.Currency.ToLower();



//            var domain = _config["AppSettings:BaseUrl"];

//            var sessionOptions = new SessionCreateOptions
//            {
//                PaymentMethodTypes = new List<string> { "card" },
//                LineItems = new List<SessionLineItemOptions>
//        {
//            new SessionLineItemOptions
//            {
//                PriceData = new SessionLineItemPriceDataOptions
//                {
//                    UnitAmountDecimal = (long)(totalPrice * 100), // price in cents
//                    Currency = currency,
//                    ProductData = new SessionLineItemPriceDataProductDataOptions
//                    {
//                        Name = $"Trip to {package.Destination}",
//                        Description = $"Hotel: {package.Hotel}, Dates: {package.Arrival:yyyy-MM-dd} to {package.Departure:yyyy-MM-dd}"
//                    }
//                },
//                Quantity = 1
//            }
//        },
//                Mode = "payment",
//                SuccessUrl = $"{domain}/booking-success?bookingId={booking.Id}",
//                CancelUrl = $"{domain}/booking-cancel?bookingId={booking.Id}",
//                Metadata = new Dictionary<string, string>
//        {
//            { "bookingId", booking.Id }
//        }
//            };

//            var service = new SessionService();
//            var session = await service.CreateAsync(sessionOptions);

//            return new BookingResponseDto
//            {
//                Success = true,
//                BookingId = booking.Id,
//                RedirectUrl = session.Url!
//            };
//        }


//        public async Task<PaymentResponseDto> ConfirmPaymentAsync(PaymentResultDto payment)
//        {
//            var booking = await _db.Bookings.FindAsync(payment.BookingId);
//            if (booking == null) return new PaymentResponseDto { Success = false, Message = "Booking not found." };

//            if (payment.Status == "success")
//            {
//                booking.PaymentStatus = "Confirmed";
//                var package = await _db.TravelPackages.FindAsync(booking.TravelPackageId);
//                if (package != null) package.IsBooked = true;

//                await _db.SaveChangesAsync();
//                return new PaymentResponseDto { Success = true, Message = "Booking confirmed." };
//            }

//            booking.PaymentStatus = "PaymentFailed";
//            await _db.SaveChangesAsync();
//            return new PaymentResponseDto { Success = false, Message = "Payment failed." };
//        }

//        public Task<PaymentResponseDto> RetryPaymentAsync(RetryPaymentDto retry)
//        {
//            // Stripe handles retries by reopening the checkout session or using customer portal
//            return Task.FromResult(new PaymentResponseDto
//            {
//                Success = false,
//                Message = "Retry payment not implemented. Please start a new booking."
//            });
//        }
//    }
//}

using GoTorz.Api.Adapters;
using GoTorz.Api.Data;
using GoTorz.Api.Services;
using GoTorz.Shared.DTOs;
using GoTorz.Shared.Models;
using Microsoft.EntityFrameworkCore;

public class BookingService : IBookingService
{
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _config;
    private readonly IPaymentAdapter _paymentAdapter;

    public BookingService(ApplicationDbContext db, IConfiguration config, IPaymentAdapter paymentAdapter)
    {
        _db = db;
        _config = config;
        _paymentAdapter = paymentAdapter;
    }

    public async Task<BookingResponseDto> SubmitCustomerInfoAsync(BookingRequestDto request)
    {
        try
        {
            var package = await _db.TravelPackages
                .Include(p => p.Hotel)
                .Include(p => p.OutboundFlight)
                .Include(p => p.ReturnFlight)
                .FirstOrDefaultAsync(p => p.TravelPackageId == request.PackageId);

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
            };

            await _db.Bookings.AddAsync(booking);
            await _db.SaveChangesAsync();

            decimal totalPrice = package.Price;
            var currency = string.IsNullOrWhiteSpace(request.Currency)
                ? (package.Currency?.ToLower() ?? "eur")
                : request.Currency.ToLower();

            string productName = $"Trip to {package.Destination}";
            string description = $"Hotel: {package.Hotel.Name}, Dates: {package.Arrival:yyyy-MM-dd} to {package.Departure:yyyy-MM-dd}";

            var redirectUrl = await _paymentAdapter.CreateCheckoutSessionAsync(
                booking.Id, totalPrice, currency, productName, description);

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
        var package = await _db.TravelPackages.FindAsync(packageId);
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
        var booking = await _db.Bookings.FindAsync(payment.BookingId);
        if (booking == null)
            return new PaymentResponseDto { Success = false, Message = "Booking not found." };

        if (payment.Status == "success")
        {
            booking.PaymentStatus = "Confirmed";
            var package = await _db.TravelPackages.FindAsync(booking.TravelPackageId);
            if (package != null) package.IsBooked = true;
            await _db.SaveChangesAsync();

            return new PaymentResponseDto { Success = true, Message = "Booking confirmed." };
        }

        booking.PaymentStatus = "PaymentFailed";
        await _db.SaveChangesAsync();

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
        var query = _db.Bookings.Include(b => b.TravelPackage).AsQueryable();

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

        return await query.Select(b => new BookingDto
        {
            Id = b.Id,
            Email = b.Email,
            CustomerName = b.FullName,
            OrderDate = b.OrderDate,
            Status = b.PaymentStatus,
            Arrival = b.TravelPackage.Arrival,
            Departure = b.TravelPackage.Departure
        }).ToListAsync();
    }

    public async Task<bool> CancelBookingAsync(string bookingId)
    {
        var booking = await _db.Bookings.FindAsync(bookingId);
        if (booking == null) return false;

        _db.Bookings.Remove(booking);
        await _db.SaveChangesAsync();
        return true;
    }
}

