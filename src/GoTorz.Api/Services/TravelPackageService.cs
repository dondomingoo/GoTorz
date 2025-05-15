using GoTorz.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTorz.Api.Services
{
    public class TravelPackageService : ITravelPackageService
    {
        private readonly ITravelPackageRepository _repository;

        public TravelPackageService(ITravelPackageRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TravelPackage>> GetAllPackagesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<TravelPackage?> GetPackageByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task CreatePackageAsync(TravelPackage package)
        {
            await _repository.AddAsync(package);
            await _repository.SaveChangesAsync();
        }

        public async Task DeletePackageAsync(string id)
        {
            await _repository.DeleteAsync(id);
            await _repository.SaveChangesAsync();
        }

        // Searches for travel packages based on filters
        public async Task<IEnumerable<TravelPackage>> GetTravelPackagesAsync(string? destination, DateTime? arrivalDate, DateTime? departureDate)
        {
            var packages = (await _repository.GetAllAsync())
                .Where(x => !x.IsBooked && x.Arrival.Date > DateTime.Now)
                
                .AsQueryable();

            if (!string.IsNullOrEmpty(destination))
            {
                packages = packages.Where(x => x.Destination.Contains(destination, StringComparison.OrdinalIgnoreCase));
            }

            if (arrivalDate.HasValue)
            {
                packages = packages.Where(x => x.Arrival.Date == arrivalDate.Value.Date);
            }

            if (departureDate.HasValue)
            {
                packages = packages.Where(x => x.Departure.Date == departureDate.Value.Date);
            }

            return await Task.FromResult(packages.ToList());
        }
    }
}
