using GoTorz.Shared.Models;

namespace GoTorz.Client.Services.Interfaces
{
    public interface ITravelService
    {
        Task<bool> CreateTravelPackageAsync(TravelPackage package);
    }
}
