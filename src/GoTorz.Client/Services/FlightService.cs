using GoTorz.Client.Services.Interfaces;
using GoTorz.Shared.DTOs.Travelplanner;
using System.Net.Http.Json;

namespace GoTorz.Client.Services
{
    public class FlightService : IFlightService
    {
        private readonly HttpClient _http;
        private readonly IClientAuthService _authService;

        public FlightService(HttpClient http, IClientAuthService authService)
        {
            _http = http;
            _authService = authService;
        }

        public async Task<FlightSearchResultDto?> SearchFlightsAsync(string fromAirport, string toAirportId, DateTime departure, DateTime returnDate, int adults, List<int> childrenAges)
        {
            try
            {
                string children = string.Join(",", childrenAges);
                string url = $"/api/flightsearch/search-flights?fromId={fromAirport}&toId={toAirportId}&departureDate={departure:yyyy-MM-dd}&returnDate={returnDate:yyyy-MM-dd}&adults={adults}&children={Uri.EscapeDataString(children)}";

                var request = await _authService.CreateAuthorizedRequest(HttpMethod.Get, url);
                if (request == null)
                {
                    Console.WriteLine("FlightService error: Unauthorized (no token).");
                    return null;
                }

                var response = await _http.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"FlightService error: {response.StatusCode}");
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<FlightSearchResultDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("FlightService error: " + ex.Message);
                return null;
            }
        }
    }
}
