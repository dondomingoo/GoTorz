using System.Net.Http.Json;
using GoTorz.Client.Services.Interfaces;
using GoTorz.Shared.DTOs.Booking;
using GoTorz.Shared.Models;

namespace GoTorz.Client.Services
{
    public class BookingHistoryService : IBookingHistoryservice
    {
        private readonly HttpClient _httpClient;
        private readonly IClientAuthService _authService;

        public BookingHistoryService(HttpClient httpClient, IClientAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        public async Task<List<BookingDto>> GetBookingHistoryAsync(string? userId, string? bookingID, DateTime? arrivalDate, DateTime? departureDate, DateTime? orderDate, string? email)
        {
            try
            {
                string query = $"?userId={userId}&bookingID={bookingID}&arrivalDate={arrivalDate?.ToString("yyyy-MM-dd")}&departureDate={departureDate?.ToString("yyyy-MM-dd")}&orderDate={orderDate?.ToString("yyyy-MM-dd")}&email={email}";

                var request = await _authService.CreateAuthorizedRequest(HttpMethod.Get, $"api/booking/all{query}");
                if (request == null)
                {
                    Console.WriteLine("BookingHistoryService error: Unauthorized (no token).");
                    return new();
                }

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"BookingHistoryService error: {response.StatusCode}");
                    return new();
                }

                var bookings = await response.Content.ReadFromJsonAsync<List<BookingDto>>();
                return bookings ?? new();
            }
            catch (Exception ex)
            {
                Console.WriteLine("BookingHistoryService error: " + ex.Message);
                return new();
            }
        }

        public async Task<bool> CancelBookingAsync(string bookingId)
        {
            try
            {
                var request = await _authService.CreateAuthorizedRequest(HttpMethod.Delete, $"api/booking/{bookingId}");
                if (request == null)
                {
                    Console.WriteLine("BookingHistoryService error: Unauthorized (no token).");
                    return false;
                }

                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("BookingHistoryService error: " + ex.Message);
                return false;
            }
        }
    }
}
