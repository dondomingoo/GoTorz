using GoTorz.Shared.DTOs;

namespace GoTorz.Client.Services.Interfaces
{

    public interface IHotelService
    {
        Task<List<HotelDto>> SearchHotelsAsync(string destId, DateTime checkin, DateTime checkout, int adults, List<int> children);
    }

}
