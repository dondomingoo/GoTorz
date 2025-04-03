using GoTorz.Shared.DTOs;

public interface IHotelService
{
    Task<List<HotelDto>> SearchHotelsAsync(string destId, DateTime checkin, DateTime checkout, int adults, List<int> children);
}
