using GoTorz.Shared.DTOs.Travelplanner;

namespace GoTorz.Api.Services
{
    public interface IHotelService
    {
        Task<List<HotelDto>> SearchHotelsAsync(string destId, string checkin, string checkout, int adults, string children);
    }
}
