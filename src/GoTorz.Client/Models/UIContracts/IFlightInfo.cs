namespace GoTorz.Client.Models.UIContracts
{
    /// <summary>
    /// Common flight data used for display in UI components.
    /// Implemented by both DTOs and models.
    /// </summary>
    public interface IFlightInfo
    {
        string FlightNumber { get; }
        string Departure { get; }
        string Destination { get; }
        DateTime DepartureTime { get; }
        DateTime ArrivalTime { get; }
    }
}