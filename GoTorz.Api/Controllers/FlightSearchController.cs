using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GoTorz.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightSearchController : ControllerBase
    {
        private readonly string ApiKey = "add6dbc133mshd0ee729a506ca11p1321dcjsn299739cd567f";

        [HttpGet("search-flights")]
        public async Task<IActionResult> SearchFlights(
            [FromQuery] string fromId,
            [FromQuery] string toId,
            [FromQuery] string departureDate,
            [FromQuery] string returnDate,
            [FromQuery] int adults = 1,
            [FromQuery] string children = "")
        {
            try
            {
                // Format legs JSON correctly with double quotes
                string legsJson = $"[{{\"fromId\":\"{fromId}\",\"toId\":\"{toId}\",\"date\":\"{departureDate}\"}},{{\"fromId\":\"{toId}\",\"toId\":\"{fromId}\",\"date\":\"{returnDate}\"}}]";
                string encodedLegs = Uri.EscapeDataString(legsJson);
                string encodedChildren = string.IsNullOrWhiteSpace(children) || children == "0" ? "" : Uri.EscapeDataString(children);

                string url = $"https://booking-com15.p.rapidapi.com/api/v1/flights/searchFlightsMultiStops" +
             $"?legs={encodedLegs}&pageNo=1&adults={adults}&children={encodedChildren}" +
             $"&sort=BEST&cabinClass=ECONOMY&currency_code=AED&location=US";


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

                using var client = new HttpClient();
                var response = await client.SendAsync(request);

                var json = await response.Content.ReadAsStringAsync();

                // Check for embedded error
                var parsed = JsonDocument.Parse(json);
                if (parsed.RootElement.TryGetProperty("data", out var dataNode) &&
                    dataNode.TryGetProperty("error", out var errorNode))
                {
                    var errorCode = errorNode.GetProperty("code").GetString();
                    var requestId = errorNode.GetProperty("requestId").GetString();
                    return BadRequest(new { error = errorCode, requestId });
                }

                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

}
