using GoTorz.Client.Services.Interfaces;
using GoTorz.Shared.DTOs.Travelplanner;
using System.Net.Http.Json;

namespace GoTorz.Client.Services
{
    public class HotelService : IHotelService
    {
        private readonly HttpClient _http;
        private readonly IClientAuthService _authService;

        public HotelService(HttpClient http, IClientAuthService authService)
        {
            _http = http;
            _authService = authService;
        }

        public async Task<List<HotelDto>> SearchHotelsAsync(string destId, DateTime checkin, DateTime checkout, int adults, List<int> children)
        {
            try
            {
                string childrenAges = string.Join(",", children);
                string url = $"api/hotels/search-hotels?destId={destId}&checkin={checkin:yyyy-MM-dd}&checkout={checkout:yyyy-MM-dd}&adults={adults}&children={childrenAges}";

                var request = await _authService.CreateAuthorizedRequest(HttpMethod.Get, url);
                if (request == null)
                {
                    Console.WriteLine("HotelService error: Unauthorized (no token).");
                    return new();
                }

                var response = await _http.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"HotelService error: {response.StatusCode}");
                    return new();
                }

                var hotels = await response.Content.ReadFromJsonAsync<List<HotelDto>>();
                return hotels ?? new();
            }
            catch (Exception ex)
            {
                Console.WriteLine("HotelService error: " + ex.Message);
                return new();
            }
        }
    }
}
