using GoTorz.Client.Services.Interfaces;
using GoTorz.Shared.Models;
using System.Net.Http.Json;

namespace GoTorz.Client.Services
{
    public class TravelService : ITravelService
    {
        private readonly HttpClient _http;

        public TravelService(HttpClient http)
        {
            _http = http;
        }

        public async Task<bool> CreateTravelPackageAsync(TravelPackage package)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("/api/TravelPackages", package);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("TravelService error: " + ex.Message);
                return false;
            }
        }
    }
}
