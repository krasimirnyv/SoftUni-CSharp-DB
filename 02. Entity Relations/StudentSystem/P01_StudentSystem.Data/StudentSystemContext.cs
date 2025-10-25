using Microsoft.EntityFrameworkCore;

namespace P01_StudentSystem.Data;

using Models;
using static Commons.ApplicationConstants;

public class StudentSystemContext : DbContext
{
    public StudentSystemContext()
    {
        
    }

    public StudentSystemContext(DbContextOptions<StudentSystemContext> options)
        : base(options)
    {
        
    }
    
    public virtual DbSet<Student> Students { get; set; } = null!;
    
    public virtual DbSet<Course> Courses { get; set; } = null!;
    
    public virtual DbSet<Resource> Resources { get; set; } = null!;
    
    public virtual DbSet<Homework> Homeworks { get; set; } = null!;
    
    public virtual DbSet<StudentCourse> StudentsCourses { get; set; } = null!;

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
        builder.Entity<StudentCourse>(e =>
        {
            e.HasKey(sc => new { sc.StudentId, sc.CourseId });
        });
        
        base.OnModelCreating(builder);
    }
}