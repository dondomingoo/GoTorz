using GoTorz.Api.Data;
using GoTorz.Api.Migrations;
using GoTorz.Shared.DTOs;
using GoTorz.Shared.Models;
using Microsoft.Extensions.Configuration;
using Stripe.Checkout;

namespace GoTorz.Api.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;

        public BookingService(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<TravelPackage?> InitiateBookingAsync(string packageId)
        {
            var package = await _db.TravelPackages.FindAsync(packageId);
            return package?.IsBooked == false ? package : null;
        }

        public async Task<BookingResponseDto> SubmitCustomerInfoAsync(BookingRequestDto request)
        {
            var package = await _db.TravelPackages.FindAsync(request.PackageId);
            if (package == null || package.IsBooked)
                return new BookingResponseDto { Success = false, Message = "Package not available." };

            var booking = new Booking
            {
                Id = Guid.NewGuid().ToString(),
                TravelPackageId = request.PackageId,
                FullName = request.FullName,
                Email = request.Email,
                PassportNumber = request.PassportNumber,
                PaymentStatus = "Pending"
            };

            await _db.Bookings.AddAsync(booking);
            await _db.SaveChangesAsync();

            int totalPeople = 1 + (request.Travellers?.Count ?? 0);
            decimal totalPrice = totalPeople * package.Price;
            var currency = string.IsNullOrWhiteSpace(request.Currency)
            ? (package.Currency?.ToLower() ?? "eur")
            : request.Currency.ToLower();



            var domain = _config["AppSettings:BaseUrl"];

            var sessionOptions = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = (long)(totalPrice * 100), // price in cents
                    Currency = currency,
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = $"Trip to {package.Destination}",
                        Description = $"Hotel: {package.Hotel}, Dates: {package.Arrival:yyyy-MM-dd} to {package.Departure:yyyy-MM-dd}"
                    }
                },
                Quantity = 1
            }
        },
                Mode = "payment",
                SuccessUrl = $"{domain}/booking-success?bookingId={booking.Id}",
                CancelUrl = $"{domain}/booking-cancel?bookingId={booking.Id}",
                Metadata = new Dictionary<string, string>
        {
            { "bookingId", booking.Id }
        }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(sessionOptions);

            return new BookingResponseDto
            {
                Success = true,
                BookingId = booking.Id,
                RedirectUrl = session.Url!
            };
        }


        public async Task<PaymentResponseDto> ConfirmPaymentAsync(PaymentResultDto payment)
        {
            var booking = await _db.Bookings.FindAsync(payment.BookingId);
            if (booking == null) return new PaymentResponseDto { Success = false, Message = "Booking not found." };

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

        public Task<PaymentResponseDto> RetryPaymentAsync(RetryPaymentDto retry)
        {
            // Stripe handles retries by reopening the checkout session or using customer portal
            return Task.FromResult(new PaymentResponseDto
            {
                Success = false,
                Message = "Retry payment not implemented. Please start a new booking."
            });
        }
    }
}