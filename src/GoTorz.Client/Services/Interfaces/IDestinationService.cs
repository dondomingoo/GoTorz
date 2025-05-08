using GoTorz.Shared.DTOs.Travelplanner;

namespace GoTorz.Client.Services.Interfaces
{
    public interface IDestinationService
    {
        Task<List<DestinationDto>> SearchDestinationsAsync(string query);
    }
}
