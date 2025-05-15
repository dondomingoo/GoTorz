using GoTorz.Client.Services.Interfaces;
using GoTorz.Shared.DTOs.Travelplanner;
using System.Net.Http.Json;

namespace GoTorz.Client.Services
{
    public class DestinationService : IDestinationService
    {
        private readonly HttpClient _http;
        private readonly IClientAuthService _authService;

        public DestinationService(HttpClient http, IClientAuthService authService)
        {
            _authService = authService;
            _http = http;
        }

        public async Task<List<DestinationDto>> SearchDestinationsAsync(string query)
        {
            try
            {
                string url = $"/api/destination?query={Uri.EscapeDataString(query)}";

                var request = await _authService.CreateAuthorizedRequest(HttpMethod.Get, url);
                if (request == null)
                {
                    Console.WriteLine("DestinationService error: Unauthorized (no token).");
                    return new();
                }

                var response = await _http.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"DestinationService error: {response.StatusCode}");
                    return new();
                }

                var destinations = await response.Content.ReadFromJsonAsync<List<DestinationDto>>();
                return destinations ?? new();
            }
            catch (Exception ex)
            {
                Console.WriteLine("DestinationService error: " + ex.Message);
                return new();
            }
        }
    }
}
