using GoTorz.Client.Services.Interfaces;
using GoTorz.Shared.DTOs.Travelplanner;
using System.Net.Http.Json;

namespace GoTorz.Client.Services
{
    public class FlightDestinationService : IFlightDestinationService
    {
        private readonly HttpClient _http;
        private readonly IClientAuthService _authService;

        public FlightDestinationService(HttpClient http, IClientAuthService authService)
        {
            _http = http;
            _authService = authService;
        }

        public async Task<List<FlightDestinationDto>> SearchFlightDestinationsAsync(string query)
        {
            try
            {
                string url = $"/api/Flights/search-flight-destinations?query={Uri.EscapeDataString(query)}";

                // Autoriseret forespørgsel
                var request = await _authService.CreateAuthorizedRequest(HttpMethod.Get, url);
                if (request == null)
                {
                    Console.WriteLine("FlightDestinationService error: Unauthorized (no token).");
                    return new();
                }

                var response = await _http.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"FlightDestinationService error: {response.StatusCode}");
                    return new();
                }

                var destinations = await response.Content.ReadFromJsonAsync<List<FlightDestinationDto>>();
                return destinations ?? new();
            }
            catch (Exception ex)
            {
                Console.WriteLine("FlightDestinationService error: " + ex.Message);
                return new();
            }
        }
    }
}
