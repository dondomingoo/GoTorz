using GoTorz.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace GoTorz.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightSearchController : ControllerBase
    {
        private readonly RapidApiSettings _rapidApiSettings;

        public FlightSearchController(IOptions<RapidApiSettings> rapidApiSettings)
        {
            _rapidApiSettings = rapidApiSettings.Value;
        }

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
                string encodedChildren = string.IsNullOrWhiteSpace(children) || children == "0" ? "" : Uri.EscapeDataString(children);

                string url = $"https://booking-com15.p.rapidapi.com/api/v1/flights/searchFlights" +
                             $"?fromId={fromId}&toId={toId}&departDate={departureDate}&returnDate={returnDate}" +
                             $"&pageNo=1&adults={adults}&children={encodedChildren}&sort=BEST&cabinClass=ECONOMY&currency_code=EUR";

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

                using var client = new HttpClient();
                var response = await client.SendAsync(request);

                var json = await response.Content.ReadAsStringAsync();

                var parsed = JsonDocument.Parse(json);
                if (!parsed.RootElement.TryGetProperty("data", out var dataNode))
                    return BadRequest("Missing data node in response.");

                var offer = dataNode.GetProperty("flightOffers").EnumerateArray().FirstOrDefault();
                if (offer.ValueKind == JsonValueKind.Undefined)
                    return Ok(null);

                var segments = offer.GetProperty("segments").EnumerateArray().ToList();
                if (segments.Count < 2) return Ok(null);

                var price = offer.GetProperty("priceBreakdown").GetProperty("total");
                string priceFormatted = $"{price.GetProperty("units").GetInt32()}.{price.GetProperty("nanos").GetInt32() / 10000000:00} EUR";

                var outbound = segments[0];
                var returnSeg = segments[1];

                var outboundDto = new OutboundFlightDto
                {
                    FlightNumber = outbound.GetProperty("legs")[0].GetProperty("flightInfo").GetProperty("flightNumber").ToString(),
                    Departure = outbound.GetProperty("departureAirport").GetProperty("cityName").GetString() ?? "",
                    Destination = outbound.GetProperty("arrivalAirport").GetProperty("cityName").GetString() ?? "",
                    DepartureTime = DateTime.Parse(outbound.GetProperty("departureTime").GetString() ?? ""),
                    ArrivalTime = DateTime.Parse(outbound.GetProperty("arrivalTime").GetString() ?? ""),
                    
                };

                var returnDto = new ReturnFlightDto
                {
                    FlightNumber = returnSeg.GetProperty("legs")[0].GetProperty("flightInfo").GetProperty("flightNumber").ToString(),
                    Departure = returnSeg.GetProperty("departureAirport").GetProperty("cityName").GetString() ?? "",
                    Destination = returnSeg.GetProperty("arrivalAirport").GetProperty("cityName").GetString() ?? "",
                    DepartureTime = DateTime.Parse(returnSeg.GetProperty("departureTime").GetString() ?? ""),
                    ArrivalTime = DateTime.Parse(returnSeg.GetProperty("arrivalTime").GetString() ?? ""),
                    
                };

                var result = new FlightSearchResultDto
                {
                    OutboundFlight = outboundDto,
                    ReturnFlight = returnDto,
                    TotalPrice = priceFormatted
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

}
