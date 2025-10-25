using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace P01_HospitalDatabase.Data.Models;

using static Commons.EntityValidation.Doctor;

public class Doctor
{
    [Key]
    public int DoctorId { get; set; }
    
    [Required]
    [Unicode]
    [MaxLength(NameMaxLength)]
    public string Name { get; set; } = null!;
    
    [Required]
    [Unicode]
    [MaxLength(SpecialtyMaxLength)]
    public string Specialty { get; set; } = null!;
    
    public virtual ICollection<Visitation> Visitations { get; set; }
        = new HashSet<Visitation>();
}