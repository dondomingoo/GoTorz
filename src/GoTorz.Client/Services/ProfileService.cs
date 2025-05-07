using GoTorz.Client.Services.Interfaces;
using GoTorz.Shared.DTOs;
using System.Net.Http.Json;

namespace GoTorz.Client.Services
{
    public class ProfileService : IProfileService
    {
        private readonly HttpClient _http;

        public ProfileService(HttpClient http)
        {
            _http = http;
        }

        public async Task<(List<BookingDto> Upcoming, List<BookingDto> Past)> GetBookingsByUserAsync(string userId)
        {
            var now = DateTime.UtcNow;
            var bookings = await _http.GetFromJsonAsync<List<BookingDto>>($"api/booking/all?userId={userId}") ?? new();

            var upcoming = bookings
                .Where(b => b.Departure >= now)
                .OrderBy(b => b.Arrival)
                .ToList();

            var past = bookings
                .Where(b => b.Departure < now)
                .OrderByDescending(b => b.Departure)
                .ToList();

            return (upcoming, past);
        }

        public async Task<(bool Success, string Message)> DeleteUserAsync(string userId)
        {
            var response = await _http.DeleteAsync($"api/auth/delete/{userId}");
            var msg = await response.Content.ReadAsStringAsync();
            return (response.IsSuccessStatusCode, msg);
        }
    }
}
