using GoTorz.Client.Services.Interfaces;
using GoTorz.Shared.DTOs;
using System.Net.Http.Json;

namespace GoTorz.Client.Services
{
    public class FlightService : IFlightService
    {
        private readonly HttpClient _http;

        public FlightService(HttpClient http)
        {
            _http = http;
        }

        public async Task<FlightSearchResultDto?> SearchFlightsAsync(string fromAirport, string toAirportId, DateTime departure, DateTime returnDate, int adults, List<int> childrenAges)
        {
            string children = string.Join(",", childrenAges);
            var url = $"/api/flightsearch/search-flights?fromId={fromAirport}&toId={toAirportId}&departureDate={departure:yyyy-MM-dd}&returnDate={returnDate:yyyy-MM-dd}&adults={adults}&children={Uri.EscapeDataString(children)}";

            try
            {
                return await _http.GetFromJsonAsync<FlightSearchResultDto>(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine("FlightService error: " + ex.Message);
                return null;
            }
        }
    }
}
