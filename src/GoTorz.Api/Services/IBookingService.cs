using GoTorz.Api.Data;
using GoTorz.Shared.DTOs;
using GoTorz.Shared.Models;

namespace GoTorz.Api.Services
{
    public interface IBookingService
    {
        Task<TravelPackage?> InitiateBookingAsync(string packageId);
        Task<BookingResponseDto> SubmitCustomerInfoAsync(BookingRequestDto request);
        Task<PaymentResponseDto> ConfirmPaymentAsync(PaymentResultDto payment);
        Task<PaymentResponseDto> RetryPaymentAsync(RetryPaymentDto retry);
    }
}