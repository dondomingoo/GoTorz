using GoTorz.Api.Adapters;
using GoTorz.Shared.DTOs.Travelplanner;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace GoTorz.Api.Services
{
    public class FlightService : IFlightService
    {
        private readonly IFlightApiAdapter _flightAdapter;

        public FlightService(IFlightApiAdapter adapter)
        {
            _flightAdapter = adapter;
        }

        public Task<List<FlightDestinationDto>> SearchFlightDestinationsAsync(string query)
            => _flightAdapter.GetFlightDestinationsAsync(query);

        public Task<FlightSearchResultDto?> SearchFlightsAsync(string fromId, string toId, string departureDate, string returnDate, int adults, string children)
            => _flightAdapter.GetFlightsAsync(fromId, toId, departureDate, returnDate, adults, children);
    }
}

