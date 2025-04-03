using GoTorz.Shared.DTOs;

namespace GoTorz.Client.Services.Interfaces
{
    public interface IDestinationService
    {
        Task<List<DestinationDto>> SearchDestinationsAsync(string query);
    }
}
