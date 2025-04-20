using GoTorz.Shared.DTOs;
using GoTorz.Shared.Models;
using GoTorz.Client.Models.Adapters;
using GoTorz.Client.Models.UIContracts;

namespace GoTorz.Client.Utilities
{
    /// <summary>
    /// Converts flight DTOs and models into display-compatible objects for the UI.
    /// </summary>
    public static class FlightDisplayMapper
    {
        public static IFlightInfo FromDto(OutboundFlightDto dto)
        {
            return new OutboundFlightDisplay
            {
                FlightNumber = dto.FlightNumber,
                Departure = dto.Departure,
                Destination = dto.Destination,
                DepartureTime = dto.DepartureTime,
                ArrivalTime = dto.ArrivalTime
            };
        }

        public static IFlightInfo FromDto(ReturnFlightDto dto)
        {
            return new ReturnFlightDisplay
            {
                FlightNumber = dto.FlightNumber,
                Departure = dto.Departure,
                Destination = dto.Destination,
                DepartureTime = dto.DepartureTime,
                ArrivalTime = dto.ArrivalTime
            };
        }

        public static IFlightInfo FromModel(OutboundFlight model)
        {
            return new OutboundFlightModelDisplay
            {
                FlightNumber = model.FlightNumber,
                Departure = model.Departure,
                Destination = model.Destination,
                DepartureTime = model.DepartureTime,
                ArrivalTime = model.ArrivalTime
            };
        }

        public static IFlightInfo FromModel(ReturnFlight model)
        {
            return new ReturnFlightModelDisplay
            {
                FlightNumber = model.FlightNumber,
                Departure = model.Departure,
                Destination = model.Destination,
                DepartureTime = model.DepartureTime,
                ArrivalTime = model.ArrivalTime
            };
        }
    }
}
