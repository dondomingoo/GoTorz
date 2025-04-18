using GoTorz.Client.Services.Interfaces;
using GoTorz.Shared.DTOs;
using System.Net.Http.Json;

namespace GoTorz.Client.Services
{
    public class DestinationService : IDestinationService
    {
        private readonly HttpClient _http;

        public DestinationService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<DestinationDto>> SearchDestinationsAsync(string query)
        {
            try
            {
                return await _http.GetFromJsonAsync<List<DestinationDto>>($"/api/destination?query={Uri.EscapeDataString(query)}") ?? new();
            }
            catch (Exception ex)
            {
                Console.WriteLine("DestinationService error: " + ex.Message);
                return new();
            }
        }
    }
}
