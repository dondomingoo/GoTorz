using GoTorz.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace GoTorz.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DestinationController : ControllerBase
    {
        private readonly RapidApiSettings _rapidApiSettings;

        public DestinationController(IOptions<RapidApiSettings> rapidApiSettings)
        {
            _rapidApiSettings = rapidApiSettings.Value;
        }

        [HttpGet]
        public async Task<IActionResult> SearchDestination([FromQuery] string query = "man")
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query is required.");
            }

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

            try
            {
                using var client = new HttpClient();
                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Error retrieving destination data.");
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

                return Ok(destinations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
