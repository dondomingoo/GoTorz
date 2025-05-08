using GoTorz.Shared.DTOs.Travelplanner;

namespace GoTorz.Api.Adapters
{
    public interface IFlightApiAdapter
    {
        Task<List<FlightDestinationDto>> GetFlightDestinationsAsync(string query);
        Task<FlightSearchResultDto?> GetFlightsAsync(string fromId, string toId, string departureDate, string returnDate, int adults, string children);
    }
}
