using GoTorz.Client.Services.Interfaces;
using GoTorz.Shared.Models;
using System.Net.Http.Json;
using System.Reflection.PortableExecutable;

namespace GoTorz.Client.Services
{
    public class TravelService : ITravelService
    {
        private readonly HttpClient _http;
        private readonly IClientAuthService _authService;

        public TravelService(HttpClient http, IClientAuthService authService)
        {
            _http = http;
            _authService = authService;
        }

        public async Task<bool> CreateTravelPackageAsync(TravelPackage package)
        {
            try
            {
                //Ask AuthService to create a request with Authorization header
                var request = await _authService.CreateAuthorizedRequest(HttpMethod.Post, "/api/TravelPackages");

                if (request == null)
                {
                    // Token is missing/invalid
                    Console.WriteLine("TravelService error: Unauthorized (no token).");
                    return false;
                }

                // Add JSON content to request (manually, since we're not using PostAsJsonAsync now)
                request.Content = JsonContent.Create(package);

                // Send the authorized request
                var response = await _http.SendAsync(request);
                //var response = await _http.PostAsJsonAsync("/api/TravelPackages", package);

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
