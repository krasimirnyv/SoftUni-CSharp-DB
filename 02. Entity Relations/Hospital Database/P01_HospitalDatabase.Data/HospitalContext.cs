using Microsoft.EntityFrameworkCore;
using P01_HospitalDatabase.Data.Models;

namespace P01_HospitalDatabase.Data;

using static Commons.ApplicationConstants;

public class HospitalContext : DbContext
{
    public HospitalContext()
    {
        
    }

    public HospitalContext(DbContextOptions<HospitalContext> options)
    {
        
    }
    
    public virtual DbSet<Patient> Patients { get; set; } = null!;
    
    public virtual DbSet<Visitation> Visitations { get; set; } = null!;
    
    public virtual DbSet<Diagnose> Diagnoses { get; set; } = null!;
    
    public virtual DbSet<Medicament> Medicaments { get; set; } = null!;
    
    public virtual DbSet<PatientMedicament> PatientsMedicaments { get; set; } = null!;
    
    public virtual DbSet<Doctor> Doctors { get; set; } = null!;
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }
        
        base.OnConfiguring(optionsBuilder);
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<PatientMedicament>(e =>
        {
            e.HasKey(pm => new { pm.PatientId, pm.MedicamentId });
        });
        
        base.OnModelCreating(builder);
    }
}