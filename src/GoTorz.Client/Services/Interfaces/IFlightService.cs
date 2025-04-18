using GoTorz.Shared.DTOs;

namespace GoTorz.Client.Services.Interfaces
{
    public interface IFlightService
    {
        Task<FlightSearchResultDto?> SearchFlightsAsync(string fromAirport, string toAirportId, DateTime departure, DateTime returnDate, int adults, List<int> childrenAges);
    }
}
