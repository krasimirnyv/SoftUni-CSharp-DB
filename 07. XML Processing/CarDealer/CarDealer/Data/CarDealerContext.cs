using Microsoft.EntityFrameworkCore;

namespace CarDealer.Data
{
    using Models;
    
    public class CarDealerContext : DbContext
    {
        public CarDealerContext()
        {
            
        }

        public CarDealerContext(DbContextOptions options)
            : base(options)
        {
            
        }

        public virtual DbSet<Car> Cars { get; set; } = null!;
        
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        
        public virtual DbSet<Part> Parts { get; set; } = null!;
        
        public virtual DbSet<PartCar> PartsCars { get; set; } = null!;
        
        public virtual DbSet<Sale> Sales { get; set; } = null!;
        
        public virtual DbSet<Supplier> Suppliers { get; set; } = null!;

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
            
            modelBuilder.Entity<PartCar>(entity =>
            {
                entity
                    .HasKey(pc => new { pc.CarId, pc.PartId });
            });

            modelBuilder.Entity<Car>(entity =>
            {
                entity
                    .HasKey(c => c.Id)
                    .HasName("PK_Cars");
                
                entity
                    .Property(c => c.Make)
                    .HasMaxLength(150)
                    .IsRequired();
                
                entity
                    .Property(c => c.Model)
                    .HasMaxLength(150)
                    .IsRequired();
                
                entity
                    .HasMany(c => c.Sales)
                    .WithOne(s => s.Car)
                    .HasForeignKey(s => s.CarId);

                entity
                    .HasMany(c => c.PartsCars)
                    .WithOne(pc => pc.Car)
                    .HasForeignKey(pc => pc.CarId);
            });

            modelBuilder.Entity<Part>(entity =>
            {
                entity
                    .HasKey(p => p.Id)
                    .HasName("PK_Parts");
            
                entity
                    .Property(p => p.Name)
                    .HasMaxLength(150)
                    .IsRequired();
            
                entity
                    .Property(p => p.Price)
                    .HasColumnType("DECIMAL(13, 6)");
            
                entity
                    .HasMany(p => p.PartsCars)
                    .WithOne(pc => pc.Part)
                    .HasForeignKey(pc => pc.PartId);
            });
            
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity
                    .HasKey(s => s.Id)
                    .HasName("PK_Suppliers");
            
                entity
                    .Property(s => s.Name)
                    .HasMaxLength(200)
                    .IsRequired();
            
                entity
                    .HasMany(s => s.Parts)
                    .WithOne(p => p.Supplier)
                    .HasForeignKey(p => p.SupplierId);
            });
            
            modelBuilder.Entity<Customer>(entity =>
            {
                entity
                    .HasKey(c => c.Id)
                    .HasName("PK_Customers");
            
                entity
                    .Property(c => c.Name)
                    .HasMaxLength(200)
                    .IsRequired();
            
                entity
                    .HasMany(c => c.Sales)
                    .WithOne(s => s.Customer)
                    .HasForeignKey(s => s.CustomerId);
            });
            
            modelBuilder.Entity<Sale>(entity =>
            {
                entity
                    .HasKey(s => s.Id)
                    .HasName("PK_Sales");
            
                entity
                    .Property(s => s.Discount)
                    .HasColumnType("DECIMAL(11, 6)");
            });
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
