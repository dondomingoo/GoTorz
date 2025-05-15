using GoTorz.Client.Services.Interfaces;
using GoTorz.Shared.DTOs.Booking;
using System.Net.Http.Json;

namespace GoTorz.Client.Services
{
    public class ProfileService : IProfileService
    {
        private readonly HttpClient _http;
        private readonly IClientAuthService _authService;

        public ProfileService(HttpClient http, IClientAuthService authService)
        {
            _http = http;
            _authService = authService;
        }

        public async Task<(List<BookingDto> Upcoming, List<BookingDto> Past)> GetBookingsByUserAsync(string userId)
        {
            try
            {
                var request = await _authService.CreateAuthorizedRequest(HttpMethod.Get, $"api/booking/all?userId={userId}");

                if (request == null)
                {
                    Console.WriteLine("ProfileService error: Unauthorized (no token).");
                    return (new(), new());
                }

                var response = await _http.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return (new(), new());

                var bookings = await response.Content.ReadFromJsonAsync<List<BookingDto>>() ?? new();
                var now = DateTime.UtcNow;

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
            catch (Exception ex)
            {
                Console.WriteLine("ProfileService error: " + ex.Message);
                return (new(), new());
            }
        }

        public async Task<(bool Success, string Message)> DeleteUserAsync(string userId)
        {
            try
            {
                var request = await _authService.CreateAuthorizedRequest(HttpMethod.Delete, $"api/auth/delete/{userId}");

                if (request == null)
                    return (false, "Unauthorized (no token)");

                var response = await _http.SendAsync(request);
                var msg = await response.Content.ReadAsStringAsync();

                return (response.IsSuccessStatusCode, msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ProfileService error: " + ex.Message);
                return (false, ex.Message);
            }
        }
    }
}
