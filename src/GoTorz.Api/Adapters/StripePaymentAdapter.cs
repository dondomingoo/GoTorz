using GoTorz.Api.Adapters;
using GoTorz.Shared.DTOs.Booking;
using Microsoft.Extensions.Configuration;
using Stripe.Checkout;

public class StripePaymentAdapter : IPaymentAdapter
{
    private readonly IConfiguration _config;

    public StripePaymentAdapter(IConfiguration config)
    {
        _config = config;
    }

    public async Task<string> CreateCheckoutSessionAsync(string bookingId, decimal amount, string currency, string productName, string description)
    {
        var domain = _config["AppSettings:BaseUrl"];
        if (string.IsNullOrWhiteSpace(domain))
        {
            throw new Exception("BaseUrl missing in config.");
        }

        var sessionOptions = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmountDecimal = (long)(amount * 100),
                        Currency = currency,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = productName,
                            Description = description
                        }
                    },
                    Quantity = 1
                }
            },
            Mode = "payment",
            SuccessUrl = $"{domain}/booking-success?bookingId={bookingId}",
            CancelUrl = $"{domain}/booking-cancel?bookingId={bookingId}",
            Metadata = new Dictionary<string, string>
            {
                { "bookingId", bookingId }
            }
        };

        var service = new SessionService();
        var session = await service.CreateAsync(sessionOptions);

        return session.Url!;
    }

    public Task<PaymentResponseDto> ConfirmPaymentAsync(PaymentResultDto payment)
    {
        // Optional: If you want to use webhooks or verify payment session here
        return Task.FromResult(new PaymentResponseDto
        {
            Success = payment.Status == "success",
            Message = payment.Status == "success" ? "Payment confirmed." : "Payment failed."
        });
    }
}
