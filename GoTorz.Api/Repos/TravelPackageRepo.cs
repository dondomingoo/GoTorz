using GoTorz.Api.Data;
using GoTorz.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoTorz.Api.Repositories
{
    public class TravelPackageRepository
    {
        private readonly ApplicationDbContext _context;

        public TravelPackageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // CREATE
        public async Task<TravelPackage> AddAsync(TravelPackage travelPackage)
        {
            _context.TravelPackages.Add(travelPackage);
            await _context.SaveChangesAsync();
            return travelPackage;
        }

        // READ - All
        public async Task<List<TravelPackage>> GetAllAsync()
        {
            return await _context.TravelPackages
                .Include(tp => tp.Hotel)
                .Include(tp => tp.OutboundFlight)
                .Include(tp => tp.ReturnFlight)
                .ToListAsync();
        }

        // READ - By ID
        public async Task<TravelPackage> GetByIdAsync(string id)
        {
            return await _context.TravelPackages
                .Include(tp => tp.Hotel)
                .Include(tp => tp.OutboundFlight)
                .Include(tp => tp.ReturnFlight)
                .FirstOrDefaultAsync(tp => tp.TravelPackageId == id);
        }

        // UPDATE
        public async Task<bool> UpdateAsync(TravelPackage updatedPackage)
        {
            var exists = await _context.TravelPackages.AnyAsync(tp => tp.TravelPackageId == updatedPackage.TravelPackageId);
            if (!exists) return false;

            _context.TravelPackages.Update(updatedPackage);
            await _context.SaveChangesAsync();
            return true;
        }

        // DELETE
        public async Task<bool> DeleteAsync(string id)
        {
            var travelPackage = await _context.TravelPackages.FindAsync(id);
            if (travelPackage == null) return false;

            _context.TravelPackages.Remove(travelPackage);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
