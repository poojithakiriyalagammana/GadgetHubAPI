using GadgetHubAPI.Data;
using GadgetHubAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GadgetHubAPI.Services
{
    public class QuotationService
    {
        private readonly GadgetHubDbContext _context;

        public QuotationService(GadgetHubDbContext context)
        {
            _context = context;
        }

        public async Task<List<Quotation>> GetAllAsync()
        {
            return await _context.Quotations
                .Include(q => q.Product)
                .Include(q => q.Distributor)
                .ToListAsync();
        }

        public async Task<Quotation?> GetByIdAsync(int id)
        {
            return await _context.Quotations
                .Include(q => q.Product)
                .Include(q => q.Distributor)
                .FirstOrDefaultAsync(q => q.QuotationId == id);
        }

        public async Task<List<Quotation>> GetByProductIdAsync(int productId)
        {
            return await _context.Quotations
                .Where(q => q.ProductId == productId)
                .Include(q => q.Distributor)
                .ToListAsync();
        }

        public async Task<Quotation> AddAsync(Quotation quotation)
        {
            _context.Quotations.Add(quotation);
            await _context.SaveChangesAsync();
            return quotation;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var quotation = await _context.Quotations.FindAsync(id);
            if (quotation == null) return false;

            _context.Quotations.Remove(quotation);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<Quotation?> UpdateAsync(Quotation quotation)
        {
            var existing = await _context.Quotations.FindAsync(quotation.QuotationId);
            if (existing == null) return null;

            existing.ProductId = quotation.ProductId;
            existing.DistributorId = quotation.DistributorId;
            existing.PricePerUnit = quotation.PricePerUnit;
            existing.Availability = quotation.Availability;
            existing.EstimatedDeliveryDays = quotation.EstimatedDeliveryDays;

            await _context.SaveChangesAsync();
            return existing;
        }
    }
}
