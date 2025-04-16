using GoTorz.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GoTorz.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController : ControllerBase
    {
        private readonly string ApiKey = "f63472ff3dmsh48a6a25a8a05abap1790dcjsn7ea0f9022bcc";

        [HttpGet("search-flight-destinations")]
        public async Task<IActionResult> SearchFlightDestinations([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query is required.");

            string url = $"https://booking-com15.p.rapidapi.com/api/v1/flights/searchDestination?query={query}";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Headers =
            {
                { "x-rapidapi-key", ApiKey },
                { "x-rapidapi-host", "booking-com15.p.rapidapi.com" },
            },
            };

            try
            {
                using var client = new HttpClient();
                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "Error retrieving flight destinations.");

                var json = await response.Content.ReadAsStringAsync();
                var parsed = JsonDocument.Parse(json);

                var results = parsed.RootElement
                    .GetProperty("data")
                    .EnumerateArray()
                    .Select(e => new FlightDestinationDto
                    {
                        Name = e.GetProperty("name").GetString() ?? "",
                        Id = e.GetProperty("id").GetString() ?? ""
                    })
                    .ToList();

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

}
