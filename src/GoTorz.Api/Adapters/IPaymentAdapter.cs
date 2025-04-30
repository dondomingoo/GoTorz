using GoTorz.Shared.DTOs;

namespace GoTorz.Api.Adapters
{
    public interface IPaymentAdapter
    {
        Task<string> CreateCheckoutSessionAsync(string bookingId, decimal amount, string currency, string productName, string description);
        Task<PaymentResponseDto> ConfirmPaymentAsync(PaymentResultDto payment);
    }
}
