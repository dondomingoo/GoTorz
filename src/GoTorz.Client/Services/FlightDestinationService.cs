using GoTorz.Client.Services.Interfaces;
using GoTorz.Shared.DTOs.Travelplanner;
using System.Net.Http.Json;

namespace GoTorz.Client.Services
{
    public class FlightDestinationService : IFlightDestinationService
    {
        private readonly HttpClient _http;

        public FlightDestinationService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<FlightDestinationDto>> SearchFlightDestinationsAsync(string query)
        {
            try
            {
                return await _http.GetFromJsonAsync<List<FlightDestinationDto>>(
                    $"/api/Flights/search-flight-destinations?query={Uri.EscapeDataString(query)}") ?? new();
            }
            catch (Exception ex)
            {
                Console.WriteLine("FlightDestinationService error: " + ex.Message);
                return new();
            }
        }
    }
}
