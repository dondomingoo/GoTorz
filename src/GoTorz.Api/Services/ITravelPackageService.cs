using GoTorz.Shared.Models;

namespace GoTorz.Api.Services
{
public interface ITravelPackageService
{
    Task<List<TravelPackage>> GetAllPackagesAsync();
    Task<TravelPackage?> GetPackageByIdAsync(string id);
    Task CreatePackageAsync(TravelPackage package);
    Task DeletePackageAsync(string id);
    Task<IEnumerable<TravelPackage>> GetTravelPackagesAsync(string? destination, int? numberOfTravellers, DateTime? arrivalDate, DateTime? departureDate);
    }
}
