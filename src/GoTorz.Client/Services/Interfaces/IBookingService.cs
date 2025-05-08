using GoTorz.Shared.DTOs;
using System.Threading.Tasks;

namespace GoTorz.Client.Services.Interfaces
{
    public interface IBookingService
    {
        /// <summary>
        /// Sends Bookingdata to backend and receives paymentlink.
        /// </summary>
        Task<BookingResponseDto> SubmitBookingAsync(BookingRequestDto request);

        /// <summary>
        /// Confirms payment and updates booking status.
        /// </summary>
        Task<PaymentResponseDto> ConfirmPaymentAsync(string bookingId);
    }
}
