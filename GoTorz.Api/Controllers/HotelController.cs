using GoTorz.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GoTorz.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelsController : ControllerBase
    {
        private readonly string ApiKey = "f63472ff3dmsh48a6a25a8a05abap1790dcjsn7ea0f9022bcc";

        [HttpGet("search-hotels")]
        public async Task<IActionResult> SearchHotels(
            [FromQuery] string destId,
            [FromQuery] string checkin,
            [FromQuery] string checkout,
            [FromQuery] int adults = 1,
            [FromQuery] string children = "")
        {
            if (string.IsNullOrWhiteSpace(destId))
                return BadRequest("Destination ID is required.");

            string encodedChildren = string.IsNullOrWhiteSpace(children) || children == "0" ? "" : Uri.EscapeDataString(children);

            string url = $"https://booking-com15.p.rapidapi.com/api/v1/hotels/searchHotels" +
                         $"?dest_id={destId}&search_type=CITY&arrival_date={checkin}&departure_date={checkout}" +
                         $"&adults={adults}&children_age={encodedChildren}" +
                         $"&room_qty=1&page_number=1&units=metric&temperature_unit=c" +
                         $"&languagecode=en-us&currency_code=EUR&location=US&sort_by=class_descending&review_score=reviewscorebuckets::70";

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
                var json = await response.Content.ReadAsStringAsync();
                var parsed = JsonDocument.Parse(json);

                var hotels = parsed.RootElement
                    .GetProperty("data")
                    .GetProperty("hotels")
                    .EnumerateArray()
                    .Select(h => new HotelDto
                    {
                        Name = h.GetProperty("property").GetProperty("name").GetString() ?? "",
                        Address = h.GetProperty("property").GetProperty("wishlistName").GetString() ?? "",
                        Stars = h.GetProperty("property").GetProperty("propertyClass").GetInt32(),
                        Price = h.GetProperty("property").GetProperty("priceBreakdown").GetProperty("grossPrice").GetProperty("value").ToString() + " EUR",
                        ImageUrl = h.GetProperty("property").GetProperty("photoUrls")[0].GetString() ?? ""
                    })
                    .ToList();

                return Ok(hotels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
