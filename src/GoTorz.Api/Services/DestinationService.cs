using GoTorz.Shared.DTOs;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace GoTorz.Api.Services
{
    public class DestinationService : IDestinationService
    {
        private readonly RapidApiSettings _rapidApiSettings;
        private readonly HttpClient _httpClient;

        public DestinationService(IOptions<RapidApiSettings> rapidApiSettings, HttpClient httpClient)
        {
            _rapidApiSettings = rapidApiSettings.Value;
            _httpClient = httpClient;
        }

        public async Task<List<DestinationDto>> SearchDestinationAsync(string query)
        {
            string url = $"https://booking-com15.p.rapidapi.com/api/v1/hotels/searchDestination?query={query}";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Headers =
                {
                    { "x-rapidapi-key", _rapidApiSettings.ApiKey },
                    { "x-rapidapi-host", _rapidApiSettings.Host },
                },
            };

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error retrieving destination data.");
            }

            var json = await response.Content.ReadAsStringAsync();
            var parsed = JsonDocument.Parse(json);

            var destinations = parsed.RootElement
                .GetProperty("data")
                .EnumerateArray()
                .Where(e => e.GetProperty("search_type").GetString() == "city")
                .Select(e => new DestinationDto
                {
                    Name = e.GetProperty("label").GetString() ?? "Unknown",
                    DestinationId = e.GetProperty("dest_id").GetString() ?? "",
                    ImageUrl = e.TryGetProperty("image_url", out var img) ? img.GetString() ?? "" : ""
                })
                .ToList();

            return destinations;
        }
    }
}
