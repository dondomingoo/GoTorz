using GoTorz.Shared.Models;

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
}
