using GoTorz.Shared.Models;

public interface ITravelPackageService
{
    Task<List<TravelPackage>> GetAllPackagesAsync();
    Task<TravelPackage?> GetPackageByIdAsync(string id);
    Task CreatePackageAsync(TravelPackage package);
    Task DeletePackageAsync(string id);
}
