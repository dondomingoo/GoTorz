using GoTorz.Shared.DTOs;

namespace GoTorz.Api.Adapters
{
    public interface IHotelApiAdapter
    {
        Task<List<HotelDto>> SearchHotelsAsync(
        string destId, string checkin, string checkout, int adults, string children);
    }
}
