using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GoTorz.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelsController : ControllerBase
    {
        private readonly string ApiKey = "add6dbc133mshd0ee729a506ca11p1321dcjsn299739cd567f";

        [HttpGet("search-hotels")]
        public async Task<IActionResult> SearchHotels(
            [FromQuery] string destId,
            [FromQuery] string checkin,
            [FromQuery] string checkout,
            [FromQuery] int adults = 1,
            [FromQuery] string children = "0",
            [FromQuery] int stars = 1)
        {
            if (string.IsNullOrWhiteSpace(destId))
                return BadRequest("Destination ID is required.");

            string url = $"https://booking-com15.p.rapidapi.com/api/v1/hotels/searchHotels" +
                         $"?dest_id={destId}&search_type=CITY" +
                         $"&checkin_date={checkin}&checkout_date={checkout}" +
                         $"&adults={adults}&children_age={Uri.EscapeDataString(children)}" +
                         $"&room_qty=1&page_number=1&units=metric&temperature_unit=c" +
                         $"&languagecode=en-us&currency_code=AED&location=US";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Headers =
            {
                { "x-rapidapi-key", $"{ApiKey}" },
                { "x-rapidapi-host", "booking-com15.p.rapidapi.com" },
            },
            };

            try
            {
                using var client = new HttpClient();
                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "Error retrieving hotel data.");

                var json = await response.Content.ReadAsStringAsync();
                var parsed = JsonDocument.Parse(json);

                var hotelList = parsed.RootElement
                    .GetProperty("data")
                    .EnumerateArray()
                    .Where(h =>
                        h.TryGetProperty("review_score_word", out _) &&
                        h.TryGetProperty("property_class", out var starProp) &&
                        int.TryParse(starProp.GetString(), out var s) && s >= stars)
                    .Select(h => new GoTorz.Shared.DTOs.HotelDto
                    {
                        Name = h.GetProperty("property_name").GetString() ?? "Unknown",
                        Address = h.GetProperty("address_trans").GetString() ?? "N/A",
                        Stars = int.TryParse(h.GetProperty("property_class").GetString(), out var s) ? s : 0,
                        Price = h.TryGetProperty("price_breakdown", out var price) ?
                                    price.GetProperty("gross_price").ToString() + " AED" :
                                    "N/A"
                    }).ToList();

                return Ok(hotelList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
