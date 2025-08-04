using GadgetHubAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GadgetHubAPI.Data
{
    public class GadgetHubDbContext : DbContext
    {
        public GadgetHubDbContext(DbContextOptions<GadgetHubDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Distributor> Distributors { get; set; }
        public DbSet<Quotation> Quotations { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Order relationships
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany()
                .HasForeignKey(o => o.CustomerId);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId);

            // Configure OrderItem relationships
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId);

            // Configure Quotation relationships
            modelBuilder.Entity<Quotation>()
                .HasOne(q => q.Product)
                .WithMany()
                .HasForeignKey(q => q.ProductId);

            modelBuilder.Entity<Quotation>()
                .HasOne(q => q.Distributor)
                .WithMany()
                .HasForeignKey(q => q.DistributorId);

            base.OnModelCreating(modelBuilder);
        }
    }
}