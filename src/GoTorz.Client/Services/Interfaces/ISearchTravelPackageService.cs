using GoTorz.Shared.Models;

namespace GoTorz.Client.Services.Interfaces
{
    public interface ISearchTravelPackageService
    {
        Task<List<TravelPackage>> GetTravelPackagesAsync(string? destination, int? numberOfTravellers, DateTime? arrivalDate, DateTime? departureDate);
    }
}
