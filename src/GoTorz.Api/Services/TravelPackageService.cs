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

        /// <summary>
        /// Search for travel packages based on destination, number of travellers, arrival date, and departure date.
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="numberOfTravellers"></param>
        /// <param name="arrivalDate"></param>
        /// <param name="departureDate"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TravelPackage>> GetTravelPackagesAsync(string? destination, int? numberOfTravellers, DateTime? arrivalDate, DateTime? departureDate)
        {
            var packages = (await _repository.GetAllAsync()).AsQueryable(); 

            if (!string.IsNullOrEmpty(destination))
            {
                packages = packages.Where(x => x.Destination.Contains(destination, StringComparison.OrdinalIgnoreCase));
            }

            if (numberOfTravellers.HasValue)
            {
                packages = packages.Where(x => x.NumberOfTravellers == numberOfTravellers.Value);
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
