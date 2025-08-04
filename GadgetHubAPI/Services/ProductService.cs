using GadgetHubAPI.Data;
using GadgetHubAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GadgetHubAPI.Services
{
    public class ProductService
    {
        private readonly GadgetHubDbContext _context;

        public ProductService(GadgetHubDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync() => await _context.Products.ToListAsync();
        public async Task<Product?> GetByIdAsync(int id) => await _context.Products.FindAsync(id);
        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
