using GoTorz.Shared.DTOs.Travelplanner;

namespace GoTorz.Api.Services
{
    public interface IFlightService
    {
        Task<List<FlightDestinationDto>> SearchFlightDestinationsAsync(string query);
        Task<FlightSearchResultDto?> SearchFlightsAsync(string fromId, string toId, string departureDate, string returnDate, int adults, string children);
    }
}
