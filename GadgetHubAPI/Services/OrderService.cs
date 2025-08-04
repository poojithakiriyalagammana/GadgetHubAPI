using GadgetHubAPI.Data;
using GadgetHubAPI.Models;
using Microsoft.EntityFrameworkCore;

public class OrderService
{
    private readonly GadgetHubDbContext _context;

    public OrderService(GadgetHubDbContext context) => _context = context;

    public async Task<List<Order>> GetAllAsync() =>
        await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ToListAsync();

    public async Task<Order> AddAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }
}