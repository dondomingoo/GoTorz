using GoTorz.Api.Data;
using GoTorz.Shared.Models;
using Microsoft.EntityFrameworkCore;

public class TravelPackageRepository : ITravelPackageRepository
{
    private readonly ApplicationDbContext _context;

    public TravelPackageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TravelPackage>> GetAllAsync()
    {
        return await _context.TravelPackages
            .Include(tp => tp.Hotel)
            .Include(tp => tp.OutboundFlight)
            .Include(tp => tp.ReturnFlight)
            .ToListAsync();
    }

    public async Task<TravelPackage?> GetByIdAsync(string id)
    {
        return await _context.TravelPackages
            .Include(tp => tp.Hotel)
            .Include(tp => tp.OutboundFlight)
            .Include(tp => tp.ReturnFlight)
            .FirstOrDefaultAsync(tp => tp.TravelPackageId == id);
    }

    public async Task AddAsync(TravelPackage package)
    {
        _context.TravelPackages.Add(package);
    }

    public async Task DeleteAsync(string id)
    {
        var package = await _context.TravelPackages.FindAsync(id);
        if (package != null)
            _context.TravelPackages.Remove(package);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
