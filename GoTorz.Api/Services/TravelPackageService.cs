using GoTorz.Shared.Models;
using GoTorz.Api.Repositories;

namespace GoTorz.Api.Services
{
    public class TravelPackageService : ITravelPackageService
    {
        private readonly TravelPackageRepository _travelPackageRepo;

        public TravelPackageService(TravelPackageRepository travelPackageRepo)
        {
            _travelPackageRepo = travelPackageRepo;
        }

        public async Task<IEnumerable<TravelPackage>> GetAllTravelPackagesAsync()
        {
            return await _travelPackageRepo.GetAllAsync() ?? Enumerable.Empty<TravelPackage>();
        }

        public async Task<IEnumerable<TravelPackage>> GetTravelPackagesAsync(string? destination, DateTime? arrivalDate, DateTime? departureDate)
        {
            var packages = await _travelPackageRepo.GetAllAsync() ?? Enumerable.Empty<TravelPackage>();

            if (!string.IsNullOrWhiteSpace(destination))
                packages = packages.Where(p => p.Destination.Contains(destination, StringComparison.OrdinalIgnoreCase)).ToList();

            if (arrivalDate.HasValue)
                packages = packages.Where(p => p.Arrival.Date == arrivalDate.Value.Date).ToList();

            if (departureDate.HasValue)
                packages = packages.Where(p => p.Departure.Date == departureDate.Value.Date).ToList();

            return packages;
        }

        public async Task<TravelPackage?> GetByIdAsync(string id)
        {
            return await _travelPackageRepo.GetByIdAsync(id);
        }

        public async Task<TravelPackage> CreateAsync(TravelPackage travelPackage)
        {
            return await _travelPackageRepo.AddAsync(travelPackage);
        }

        public async Task<bool> UpdateAsync(TravelPackage travelPackage)
        {
            return await _travelPackageRepo.UpdateAsync(travelPackage);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await _travelPackageRepo.DeleteAsync(id);
        }
    }
}
