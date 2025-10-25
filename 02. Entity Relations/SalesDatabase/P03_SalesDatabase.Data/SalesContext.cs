using Microsoft.EntityFrameworkCore;

namespace P03_SalesDatabase.Data;

using Models;
using static Commons.ApplicationConstants;
public class SalesContext : DbContext
{
    public SalesContext()
    {
        
    }

    public SalesContext(DbContextOptions<SalesContext> options)
    {
        
    }
    
    public virtual DbSet<Customer> Customers { get; set; } = null!;
    
    public virtual DbSet<Product> Products { get; set; } = null!;
    
    public virtual DbSet<Store> Stores { get; set; } = null!;

    public virtual DbSet<Sale> Sales { get; set; } = null!;
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }
        
        base.OnConfiguring(optionsBuilder);
    }
}