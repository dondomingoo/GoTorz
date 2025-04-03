using GoTorz.Shared.Models;

namespace GoTorz.Api.Services
{
    public interface ITravelPackageService
    {
        Task<IEnumerable<TravelPackage>> GetAllTravelPackagesAsync();
        Task<IEnumerable<TravelPackage>> GetTravelPackagesAsync(string? destination, DateTime? arrivalDate, DateTime? departureDate);
    }
}