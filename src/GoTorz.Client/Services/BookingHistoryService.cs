using System.Net.Http.Json;
using GoTorz.Client.Services.Interfaces;
using GoTorz.Shared.DTOs.Booking;
using GoTorz.Shared.Models;

namespace GoTorz.Client.Services
{
    public class BookingHistoryService : IBookingHistoryservice
    {
        private readonly HttpClient _httpClient;
        public BookingHistoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<BookingDto>> GetBookingHistoryAsync(string? userId, string? bookingID, DateTime? arrivalDate, DateTime? departureDate, DateTime? orderDate, string? email)
        {
            string query = $"?userId={userId}&bookingID={bookingID}&arrivalDate={arrivalDate?.ToString("yyyy-MM-dd")}&departureDate={departureDate?.ToString("yyyy-MM-dd")}&orderDate={orderDate?.ToString("yyyy-MM-dd")}&email={email}";

            return await _httpClient.GetFromJsonAsync<List<BookingDto>>($"api/booking/all{query}");

        }
        public async Task<bool> CancelBookingAsync(string bookingId)
        {
            var response = await _httpClient.DeleteAsync($"api/booking/{bookingId}");

            return response.IsSuccessStatusCode;
        }
    }


}