using System.Net.Http.Json;
using GoTorz.Client.Services.Interfaces;
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
        public async Task<List<Booking>> GetBookingHistoryAsync(string? userId, string? bookingID, DateTime? arrivalDate, DateTime? departureDate, DateTime? orderDate)
        {
            string query = $"?userId={userId}&bookingID={bookingID}&arrivalDate={arrivalDate?.ToString("yyyy-MM-dd")}&departureDate={departureDate?.ToString("yyyy-MM-dd")}&orderDate={orderDate?.ToString("yyyy-MM-dd")}";
            return await _httpClient.GetFromJsonAsync<List<Booking>>($"api/bookinghistory{query}");
        }
        public async Task<bool> CancelBookingAsync(string bookingId)
        {
            var response = await _httpClient.DeleteAsync($"api/bookinghistory/{bookingId}");
            return response.IsSuccessStatusCode;
        }
    }


}