using GoTorz.Client.Services.Interfaces;
using GoTorz.Shared.DTOs.Booking;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GoTorz.Client.Services
{
    public class BookingService : IBookingService
    {
        private readonly HttpClient _http;
        private readonly IClientAuthService _authService;

        public BookingService(HttpClient http, IClientAuthService authService)
        {
            _http = http;
            _authService = authService;
        }

        public async Task<BookingResponseDto> SubmitBookingAsync(BookingRequestDto requestDto)
        {
            try
            {
                var request = await _authService.CreateAuthorizedRequest(HttpMethod.Post, "api/booking/submit-info");
                if (request == null)
                {
                    Console.WriteLine("BookingService error: Unauthorized (no token).");
                    return new BookingResponseDto { Success = false, Message = "Unauthorized." };
                }

                request.Content = JsonContent.Create(requestDto);

                var response = await _http.SendAsync(request);
                return await response.Content.ReadFromJsonAsync<BookingResponseDto>() ?? new BookingResponseDto
                {
                    Success = false,
                    Message = "Unknown error occurred."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("BookingService error: " + ex.Message);
                return new BookingResponseDto { Success = false, Message = ex.Message };
            }
        }

        public async Task<PaymentResponseDto> ConfirmPaymentAsync(string bookingId)
        {
            try
            {
                var payment = new PaymentResultDto
                {
                    BookingId = bookingId,
                    Status = "success"
                };

                var request = await _authService.CreateAuthorizedRequest(HttpMethod.Post, "api/booking/confirm-payment");
                if (request == null)
                {
                    Console.WriteLine("BookingService error: Unauthorized (no token).");
                    return new PaymentResponseDto { Success = false, Message = "Unauthorized." };
                }

                request.Content = JsonContent.Create(payment);

                var response = await _http.SendAsync(request);
                return await response.Content.ReadFromJsonAsync<PaymentResponseDto>() ?? new PaymentResponseDto
                {
                    Success = false,
                    Message = "Could not confirm payment."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("BookingService error: " + ex.Message);
                return new PaymentResponseDto { Success = false, Message = ex.Message };
            }
        }
    }
}
