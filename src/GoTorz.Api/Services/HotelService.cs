using GoTorz.Api.Adapters;
using GoTorz.Api.Services;
using GoTorz.Shared.DTOs.Travelplanner;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text.Json;

namespace GoTorz.Api.Services
{
    public class HotelService : IHotelService
    {
        private readonly IHotelApiAdapter _hotelAdapter;

        public HotelService(IHotelApiAdapter rapidApiHotelAdapter)
        {
            _hotelAdapter = rapidApiHotelAdapter;
        }

        public async Task<List<HotelDto>> SearchHotelsAsync(
            string destId, string checkin, string checkout, int adults, string children)
        {
            if (string.IsNullOrWhiteSpace(destId))
                throw new Exception("Destination ID is required.");

            return await _hotelAdapter.SearchHotelsAsync(destId, checkin, checkout, adults, children);
        }
    }
}
