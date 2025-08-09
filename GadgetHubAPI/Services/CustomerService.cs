using GadgetHubAPI.Data;
using GadgetHubAPI.Models;
using Microsoft.EntityFrameworkCore;

public class CustomerService
{
    private readonly GadgetHubDbContext _context;
    public CustomerService(GadgetHubDbContext context) => _context = context;

    public Task<List<Customer>> GetAllAsync() => _context.Customers.ToListAsync();
    public Task<Customer?> GetByIdAsync(int id) => _context.Customers.FindAsync(id).AsTask();
    public async Task<Customer> AddAsync(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task<Customer?> UpdateAsync(Customer customer)
    {
        var existing = await _context.Customers.FindAsync(customer.CustomerId);
        if (existing == null) return null;

        existing.FullName = customer.FullName;
        existing.Email = customer.Email;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return false;

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return true;
    }
}
