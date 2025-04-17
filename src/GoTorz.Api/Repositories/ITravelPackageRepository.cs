using GoTorz.Shared.Models;

public interface ITravelPackageRepository
{
    Task<List<TravelPackage>> GetAllAsync();
    Task<TravelPackage?> GetByIdAsync(string id);
    Task AddAsync(TravelPackage package);
    Task DeleteAsync(string id);
    Task SaveChangesAsync();
}