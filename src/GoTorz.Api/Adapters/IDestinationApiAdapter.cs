using GoTorz.Shared.DTOs;

namespace GoTorz.Api.Adapters
{
    public interface IDestinationApiAdapter
    {
        Task<List<DestinationDto>> SearchDestinationAsync(string query);
    }
}
