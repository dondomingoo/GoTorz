using GoTorz.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTorz.Api.Services
{
    public class TravelPackageService : ITravelPackageService
    {
        // private readonly ITravelPackageRepo _travelPackageRepo; // Will be used when repo is ready
        private readonly List<TravelPackage> _packages; // Mock data for now

        public TravelPackageService()
        {
            //_travelPackageRepo = travelPackageRepo; // Uncomment when repo is ready

            // Mock data for demonstration
            _packages = new List<TravelPackage>
        {
            new TravelPackage
            {
                TravelPackageId = "1",
                Destination = "Paris",
                Arrival = DateTime.Now.AddDays(5),
                Departure = DateTime.Now.AddDays(10),
                price = "500 EUR",
                Hotel = new Hotel { Name = "Hotel Paris", Rooms = 1 }
            },
            new TravelPackage
            {
                TravelPackageId = "3",
                Destination = "Paris",
                Arrival = DateTime.Now.AddDays(10),
                Departure = DateTime.Now.AddDays(15),
                price = "1500 EUR",
                Hotel = new Hotel { Name = "Hotel Paris", Rooms = 1 }
            },
            new TravelPackage
            {
                TravelPackageId = "2",
                Destination = "Berlin",
                Arrival = DateTime.Now.AddDays(1),
                Departure = DateTime.Now.AddDays(7),
                price = "350 EUR",
                Hotel = new Hotel { Name = "Hotel Berlin", Rooms = 2 }
            }
        };
        }


        public async Task<IEnumerable<TravelPackage>> GetAllTravelPackagesAsync()
        {
            // return await _travelPackageRepo.GetAllTravelPackagesAsync(); // Uncomment when repo is ready
            return await Task.FromResult(_packages);
        }

        // Searches for travel packages based on filters
        public async Task<IEnumerable<TravelPackage>> GetTravelPackagesAsync(string? destination, DateTime? arrivalDate, DateTime? departureDate)
        {
            // var packages = await _travelPackageRepo.GetTravelPackagesAsync(destination, arrivalDate, departureDate); // Uncomment when repo is ready
            var packages = _packages.AsQueryable(); // Use mock data 

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