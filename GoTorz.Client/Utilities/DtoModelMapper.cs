using GoTorz.Shared.DTOs;
using GoTorz.Shared.Models;

namespace GoTorz.Client.Utilities
{
    public static class DtoModelMapper
    {
        public static Hotel MapToModel(HotelDto dto, DateTime checkin, DateTime checkout)
        {
            return new Hotel
            {
                Name = dto.Name,
                Address = dto.Address,
                Checkin = checkin,
                Checkout = checkout,
                Rooms = 1 
            };
        }

        public static OutboundFlight MapToModel(OutboundFlightDto dto)
        {
            return new OutboundFlight
            {
                FlightNumber = dto.FlightNumber,
                Departure = dto.Departure,
                Destination = dto.Destination,
                DepartureTime = dto.DepartureTime,
                ArrivalTime = dto.ArrivalTime
            };
        }

        public static ReturnFlight MapToModel(ReturnFlightDto dto)
        {
            return new ReturnFlight
            {
                FlightNumber = dto.FlightNumber,
                Departure = dto.Departure,
                Destination = dto.Destination,
                DepartureTime = dto.DepartureTime,
                ArrivalTime = dto.ArrivalTime
            };
        }

        public static TravelPackage CreateTravelPackage(
            string destinationName,
            DateTime arrival,
            DateTime departure,
            HotelDto hotelDto,
            OutboundFlightDto outboundDto,
            ReturnFlightDto returnDto,
            string totalPrice)
        {
            return new TravelPackage
            {
                TravelPackageId = Guid.NewGuid().ToString(),
                Destination = destinationName,
                Arrival = arrival,
                Departure = departure,
                Hotel = MapToModel(hotelDto, arrival, departure),
                OutboundFlight = MapToModel(outboundDto),
                ReturnFlight = MapToModel(returnDto),
                Price = totalPrice
            };
        }
    }
}
