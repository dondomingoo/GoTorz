using GoTorz.Shared.Models;

namespace GoTorz.Api.Services
{
    public interface ITravelPackageService
    {
        Task<IEnumerable<TravelPackage>> GetAllTravelPackagesAsync();
        Task<IEnumerable<TravelPackage>> GetTravelPackagesAsync(string? destination, DateTime? arrivalDate, DateTime? departureDate);
        Task<TravelPackage?> GetByIdAsync(string id);
        Task<TravelPackage> CreateAsync(TravelPackage travelPackage);
        Task<bool> UpdateAsync(TravelPackage travelPackage);
        Task<bool> DeleteAsync(string id);
    }

}