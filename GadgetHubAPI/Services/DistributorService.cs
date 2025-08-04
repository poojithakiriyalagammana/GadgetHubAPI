using GadgetHubAPI.Data;
using GadgetHubAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GadgetHubAPI.Services
{
    public class DistributorService
    {
        private readonly GadgetHubDbContext _context;

        public DistributorService(GadgetHubDbContext context)
        {
            _context = context;
        }

        public async Task<List<Distributor>> GetAllAsync() => await _context.Distributors.ToListAsync();

        public async Task<Distributor?> GetByIdAsync(int id) => await _context.Distributors.FindAsync(id);

        public async Task<Distributor> AddAsync(Distributor distributor)
        {
            _context.Distributors.Add(distributor);
            await _context.SaveChangesAsync();
            return distributor;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var distributor = await _context.Distributors.FindAsync(id);
            if (distributor == null) return false;

            _context.Distributors.Remove(distributor);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
