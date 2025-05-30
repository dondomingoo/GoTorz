﻿using GoTorz.Shared.DTOs.Travelplanner;

namespace GoTorz.Client.Services.Interfaces
{
    public interface IFlightDestinationService
    {
        Task<List<FlightDestinationDto>> SearchFlightDestinationsAsync(string query);
    }
}