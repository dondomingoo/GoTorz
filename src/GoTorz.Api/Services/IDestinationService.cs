using GoTorz.Shared.DTOs;

namespace GoTorz.Api.Services
{
    public interface IDestinationService
    {
        Task<List<DestinationDto>> SearchDestinationAsync(string query);
    }
}

