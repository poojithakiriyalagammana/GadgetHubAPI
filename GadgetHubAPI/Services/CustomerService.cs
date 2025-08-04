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
}
