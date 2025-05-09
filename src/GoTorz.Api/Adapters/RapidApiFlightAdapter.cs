using GoTorz.Shared.DTOs.Travelplanner;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace GoTorz.Api.Adapters
{
    public class RapidApiFlightAdapter : IFlightApiAdapter
    {
        private readonly HttpClient _httpClient;
        private readonly RapidApiSettings _rapidApiSettings;
        public RapidApiFlightAdapter(HttpClient httpClient, IOptions<RapidApiSettings> settings)
        {
            _httpClient = httpClient;
            _rapidApiSettings = settings.Value;
        }

        public async Task<List<FlightDestinationDto>> GetFlightDestinationsAsync(string query)
        {
            var url = $"https://booking-com15.p.rapidapi.com/api/v1/flights/searchDestination?query={query}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("x-rapidapi-key", _rapidApiSettings.ApiKey);
            request.Headers.Add("x-rapidapi-host", _rapidApiSettings.Host);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Flight destination search failed.");

            var json = await response.Content.ReadAsStringAsync();
            var parsed = JsonDocument.Parse(json);

            return parsed.RootElement
                .GetProperty("data")
                .EnumerateArray()
                .Select(e => new FlightDestinationDto
                {
                    Name = e.GetProperty("name").GetString() ?? "",
                    Id = e.GetProperty("id").GetString() ?? ""
                })
                .ToList();
        }

        public async Task<FlightSearchResultDto?> GetFlightsAsync(string fromId, string toId, string departureDate, string returnDate, int adults, string children)
        {
            string encodedChildren = string.IsNullOrWhiteSpace(children) || children == "0"
                ? ""
                : Uri.EscapeDataString(children);

            string url = $"https://booking-com15.p.rapidapi.com/api/v1/flights/searchFlights" +
                         $"?fromId={fromId}&toId={toId}&departDate={departureDate}&returnDate={returnDate}" +
                         $"&pageNo=1&adults={adults}&children={encodedChildren}&sort=BEST&cabinClass=ECONOMY&currency_code=EUR";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("x-rapidapi-key", _rapidApiSettings.ApiKey);
            request.Headers.Add("x-rapidapi-host", _rapidApiSettings.Host);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var parsed = JsonDocument.Parse(json);

            if (!parsed.RootElement.TryGetProperty("data", out var dataNode))
                throw new Exception("Missing data node in response.");

            var offer = dataNode.GetProperty("flightOffers").EnumerateArray().FirstOrDefault();
            if (offer.ValueKind == JsonValueKind.Undefined)
                return null;

            var segments = offer.GetProperty("segments").EnumerateArray().ToList();
            if (segments.Count < 2)
                return null;

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
                ArrivalTime = DateTime.Parse(outbound.GetProperty("arrivalTime").GetString() ?? "")
            };

            var returnDto = new ReturnFlightDto
            {
                FlightNumber = returnSeg.GetProperty("legs")[0].GetProperty("flightInfo").GetProperty("flightNumber").ToString(),
                Departure = returnSeg.GetProperty("departureAirport").GetProperty("cityName").GetString() ?? "",
                Destination = returnSeg.GetProperty("arrivalAirport").GetProperty("cityName").GetString() ?? "",
                DepartureTime = DateTime.Parse(returnSeg.GetProperty("departureTime").GetString() ?? ""),
                ArrivalTime = DateTime.Parse(returnSeg.GetProperty("arrivalTime").GetString() ?? "")
            };

            return new FlightSearchResultDto
            {
                OutboundFlight = outboundDto,
                ReturnFlight = returnDto,
                TotalPrice = priceFormatted
            };
        }
    }
}
