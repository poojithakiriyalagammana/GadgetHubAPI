using GadgetHubAPI.Data;
using GadgetHubAPI.Models;
using GadgetHubAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GadgetHubAPI.Services
{
    public class OrderService
    {
        private readonly GadgetHubDbContext _context;

        public OrderService(GadgetHubDbContext context) => _context = context;

        public async Task<List<OrderResponseDto>> GetAllAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();

            // Get all product IDs from all order items
            var allProductIds = orders
                .SelectMany(o => o.OrderItems.Select(oi => oi.ProductId))
                .Distinct()
                .ToList();

            // Get products and their current quotations in one query
            var productsWithQuotations = await _context.Products
                .Where(p => allProductIds.Contains(p.ProductId))
                .Include(p => p.Quotations)
                .ToDictionaryAsync(p => p.ProductId);

            return orders.Select(o => new OrderResponseDto
            {
                OrderId = o.OrderId,
                CustomerId = o.CustomerId,
                OrderDate = o.OrderDate,
                CustomerName = o.Customer != null ? $"{o.Customer.FullName}" : "",
                OrderItems = o.OrderItems.Select(oi =>
                {
                    if (!productsWithQuotations.TryGetValue(oi.ProductId, out var product))
                    {
                        // Fallback to stored unit price if product not found
                        return new OrderItemResponseDto
                        {
                            OrderItemId = oi.OrderItemId,
                            ProductId = oi.ProductId,
                            ProductName = oi.Product?.Name ?? "Unknown Product",
                            Quantity = oi.Quantity,
                            UnitPrice = oi.UnitPrice, // Use stored price
                            TotalPrice = oi.Quantity * oi.UnitPrice
                        };
                    }

                    var latestQuotation = product.Quotations
                        ?.OrderByDescending(q => q.QuotationId)
                        ?.FirstOrDefault();

                    // Use latest quotation price if available, otherwise use stored unit price
                    var unitPrice = latestQuotation?.PricePerUnit ?? oi.UnitPrice;

                    return new OrderItemResponseDto
                    {
                        OrderItemId = oi.OrderItemId,
                        ProductId = oi.ProductId,
                        ProductName = oi.Product?.Name ?? "",
                        Quantity = oi.Quantity,
                        UnitPrice = unitPrice,
                        TotalPrice = oi.Quantity * unitPrice
                    };
                }).ToList(),
                TotalAmount = o.OrderItems.Sum(oi =>
                {
                    if (!productsWithQuotations.TryGetValue(oi.ProductId, out var product))
                    {
                        return oi.Quantity * oi.UnitPrice; // Use stored price
                    }

                    var latestQuotation = product.Quotations
                        ?.OrderByDescending(q => q.QuotationId)
                        ?.FirstOrDefault();

                    var unitPrice = latestQuotation?.PricePerUnit ?? oi.UnitPrice;
                    return oi.Quantity * unitPrice;
                })
            }).ToList();
        }

        public async Task<OrderResponseDto?> GetByIdAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return null;

            // Get all product IDs from the order items
            var productIds = order.OrderItems.Select(oi => oi.ProductId).ToList();

            // Get products and their current quotations
            var productsWithQuotations = await _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .Include(p => p.Quotations)
                .ToDictionaryAsync(p => p.ProductId);

            return new OrderResponseDto
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                OrderDate = order.OrderDate,
                CustomerName = order.Customer != null ? $"{order.Customer.FullName}" : "",
                OrderItems = order.OrderItems.Select(oi =>
                {
                    if (!productsWithQuotations.TryGetValue(oi.ProductId, out var product))
                    {
                        // Fallback to stored unit price if product not found
                        return new OrderItemResponseDto
                        {
                            OrderItemId = oi.OrderItemId,
                            ProductId = oi.ProductId,
                            ProductName = oi.Product?.Name ?? "Unknown Product",
                            Quantity = oi.Quantity,
                            UnitPrice = oi.UnitPrice, // Use stored price
                            TotalPrice = oi.Quantity * oi.UnitPrice
                        };
                    }

                    var latestQuotation = product.Quotations
                        ?.OrderByDescending(q => q.QuotationId)
                        ?.FirstOrDefault();

                    // Use latest quotation price if available, otherwise use stored unit price
                    var unitPrice = latestQuotation?.PricePerUnit ?? oi.UnitPrice;

                    return new OrderItemResponseDto
                    {
                        OrderItemId = oi.OrderItemId,
                        ProductId = oi.ProductId,
                        ProductName = oi.Product?.Name ?? "",
                        Quantity = oi.Quantity,
                        UnitPrice = unitPrice,
                        TotalPrice = oi.Quantity * unitPrice
                    };
                }).ToList(),
                TotalAmount = order.OrderItems.Sum(oi =>
                {
                    if (!productsWithQuotations.TryGetValue(oi.ProductId, out var product))
                    {
                        return oi.Quantity * oi.UnitPrice; // Use stored price
                    }

                    var latestQuotation = product.Quotations
                        ?.OrderByDescending(q => q.QuotationId)
                        ?.FirstOrDefault();

                    var unitPrice = latestQuotation?.PricePerUnit ?? oi.UnitPrice;
                    return oi.Quantity * unitPrice;
                })
            };
        }

        public async Task<OrderResponseDto> CreateAsync(CreateOrderDto createOrderDto)
        {
            // Validate customer exists
            var customer = await _context.Customers.FindAsync(createOrderDto.CustomerId);
            if (customer == null)
                throw new ArgumentException("Customer not found");

            // Get all product IDs from the order items
            var productIds = createOrderDto.OrderItems.Select(oi => oi.ProductId).ToList();

            // Get products and their current quotations
            var productsWithQuotations = await _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .Include(p => p.Quotations)
                .ToDictionaryAsync(p => p.ProductId);

            // Validate all products exist and have quotations
            foreach (var item in createOrderDto.OrderItems)
            {
                if (!productsWithQuotations.ContainsKey(item.ProductId))
                    throw new ArgumentException($"Product with ID {item.ProductId} not found");

                if (item.Quantity <= 0)
                    throw new ArgumentException("Quantity must be greater than 0");

                var product = productsWithQuotations[item.ProductId];
                if (product.Quotations == null || !product.Quotations.Any())
                    throw new ArgumentException($"No quotations available for product {product.Name}. Cannot create order without pricing information.");

                var latestQuotation = product.Quotations.OrderByDescending(q => q.QuotationId).FirstOrDefault();
                if (latestQuotation == null || latestQuotation.PricePerUnit <= 0)
                    throw new ArgumentException($"Invalid or missing price for product {product.Name}");
            }

            // Create the order with items
            var order = new Order
            {
                CustomerId = createOrderDto.CustomerId,
                OrderDate = DateTime.UtcNow,
                OrderItems = createOrderDto.OrderItems.Select(oi =>
                {
                    var product = productsWithQuotations[oi.ProductId];
                    var latestQuotation = product.Quotations.OrderByDescending(q => q.QuotationId).First();

                    return new OrderItem
                    {
                        ProductId = oi.ProductId,
                        Quantity = oi.Quantity,
                        UnitPrice = latestQuotation.PricePerUnit
                    };
                }).ToList()
            };


            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(order.OrderId)
                ?? throw new InvalidOperationException("Failed to retrieve created order");
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}