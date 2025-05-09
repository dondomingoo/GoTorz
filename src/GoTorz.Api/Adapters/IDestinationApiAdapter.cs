using GoTorz.Shared.DTOs.Travelplanner;

namespace GoTorz.Api.Adapters
{
    public interface IDestinationApiAdapter
    {
        Task<List<DestinationDto>> SearchDestinationAsync(string query);
    }
}
