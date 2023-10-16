using Microsoft.EntityFrameworkCore;
using TeamProject.Shared.Models;
using TeamProject.Shared.User;

namespace TeamProject.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<User> Users { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CartItem>()
                .HasKey(ci => new
                {
                    ci.UserId,
                    ci.ProductId,
                    ci.ProductTypeId
                });

            modelBuilder.Entity<ProductVariant>()
                .HasKey(p => new
                {
                    p.ProductId,
                    p.ProductTypeId
                });

            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => new
                {
                    oi.OrderId,
                    oi.ProductId,
                    oi.ProductTypeId
                });
        }
    }
}
