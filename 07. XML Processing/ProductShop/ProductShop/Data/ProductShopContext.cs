using Microsoft.EntityFrameworkCore;

namespace ProductShop.Data
{
    using Models;

    public class ProductShopContext : DbContext
    {
        public ProductShopContext()
        {
            
        }

        public ProductShopContext(DbContextOptions options)
            : base(options)
        {
            
        }

        public virtual DbSet<Category> Categories { get; set; } = null!;

        public virtual DbSet<Product> Products { get; set; } = null!;

        public virtual DbSet<User> Users { get; set; } = null!;

        public virtual DbSet<CategoryProduct> CategoryProducts { get; set; } = null!;
        
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CategoryProduct>(entity =>
            {
                entity
                    .HasKey(x => new { x.CategoryId, x.ProductId });
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity
                    .HasKey(e => e.Id)
                    .HasName("PK__Users");
                
                entity
                    .Property(e => e.FirstName)
                    .HasMaxLength(150)
                    .IsRequired(false);
                
                entity
                    .Property(e => e.LastName)
                    .HasMaxLength(150)
                    .IsRequired();
                
                entity
                    .Property(e => e.Age)
                    .IsRequired(false);
                    
                entity
                    .HasMany(x => x.ProductsBought)
                    .WithOne(x => x.Buyer)
                    .HasForeignKey(x => x.BuyerId);

                entity
                    .HasMany(x => x.ProductsSold)
                    .WithOne(x => x.Seller)
                    .HasForeignKey(x => x.SellerId);
            });
            
            modelBuilder.Entity<Category>(entity =>
            {
                entity
                    .HasKey(e => e.Id)
                    .HasName("PK__Categories");
            
                entity
                    .Property(e => e.Name)
                    .HasMaxLength(200)
                    .IsRequired();
            });
            
            modelBuilder.Entity<Product>(entity =>
            {
                entity
                    .HasKey(e => e.Id)
                    .HasName("PK__Products");
                
                entity
                    .Property(e => e.Name)
                    .HasMaxLength(200)
                    .IsRequired();
            
                entity
                    .Property(e => e.Price)
                    .HasColumnType("DECIMAL(18, 6)");
            });
        }
    }
}
