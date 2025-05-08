using GoTorz.Shared.DTOs.Travelplanner;

namespace GoTorz.Api.Services
{
    public interface IDestinationService
    {
        Task<List<DestinationDto>> SearchDestinationAsync(string query);
    }
}

