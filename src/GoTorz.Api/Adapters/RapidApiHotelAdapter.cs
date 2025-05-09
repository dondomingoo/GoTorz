using GoTorz.Shared.DTOs.Travelplanner;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace GoTorz.Api.Adapters
{
    public class RapidApiHotelAdapter:IHotelApiAdapter
    {
        private readonly HttpClient _httpClient;
        private readonly RapidApiSettings _settings;

        public RapidApiHotelAdapter(HttpClient httpClient, IOptions<RapidApiSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<List<HotelDto>> SearchHotelsAsync(
            string destId, string checkin, string checkout, int adults, string children)
        {
            if (string.IsNullOrWhiteSpace(destId))
                throw new Exception("Destination ID is required.");

            string encodedChildren = string.IsNullOrWhiteSpace(children) || children == "0"
                ? ""
                : Uri.EscapeDataString(children);

            string url = $"https://booking-com15.p.rapidapi.com/api/v1/hotels/searchHotels" +
                         $"?dest_id={destId}&search_type=CITY&arrival_date={checkin}&departure_date={checkout}" +
                         $"&adults={adults}&children_age={encodedChildren}" +
                         $"&room_qty=1&page_number=1&units=metric&temperature_unit=c" +
                         $"&languagecode=en-us&currency_code=EUR&location=US&sort_by=class_descending&review_score=reviewscorebuckets::70";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("x-rapidapi-key", _settings.ApiKey);
            request.Headers.Add("x-rapidapi-host", _settings.Host);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Hotel API request failed.");

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

            return hotels;
        }
    }
}
