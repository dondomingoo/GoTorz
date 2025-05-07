using GoTorz.Client.Services.Interfaces;
using GoTorz.Shared.DTOs;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GoTorz.Client.Services
{
    public class BookingService : IBookingService
    {
        private readonly HttpClient _http;

        public BookingService(HttpClient http)
        {
            _http = http;
        }

        public async Task<BookingResponseDto> SubmitBookingAsync(BookingRequestDto request)
        {
            var response = await _http.PostAsJsonAsync("api/booking/submit-info", request);
            return await response.Content.ReadFromJsonAsync<BookingResponseDto>() ?? new BookingResponseDto
            {
                Success = false,
                Message = "Unknown error occurred."
            };
        }

        public async Task<PaymentResponseDto> ConfirmPaymentAsync(string bookingId)
        {
            var payment = new PaymentResultDto
            {
                BookingId = bookingId,
                Status = "success"
            };

            var response = await _http.PostAsJsonAsync("api/booking/confirm-payment", payment);
            return await response.Content.ReadFromJsonAsync<PaymentResponseDto>() ?? new PaymentResponseDto
            {
                Success = false,
                Message = "Could not confirm payment."
            };
        }
    }
}
