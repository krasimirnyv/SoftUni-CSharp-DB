using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace P01_HospitalDatabase.Data.Models;

using static Commons.EntityValidation.Patient;

public class Patient
{
    [Key]
    public int PatientId { get; set; }

    [Required] 
    [Unicode] 
    [MaxLength(FirstNameMaxLength)] 
    public string FirstName { get; set; } = null!;
    
    [Required]
    [Unicode]
    [MaxLength(LastNameMaxLength)]
    public string LastName { get; set; } = null!;
    
    [Required]
    [Unicode]
    [MaxLength(AddressMaxLength)]
    public string Address { get; set; } = null!;
    
    [Unicode(EmailUnicode)]
    [MaxLength(EmailMaxLength)]
    public string? Email { get; set; }
    
    public bool HasInsurance { get; set; }
    
    public virtual ICollection<Visitation> Visitations { get; set; }
        = new HashSet<Visitation>();
    
    public virtual ICollection<Diagnose> Diagnoses { get; set; }
        = new HashSet<Diagnose>();
    
    public virtual ICollection<PatientMedicament> Prescriptions { get; set; }
        = new HashSet<PatientMedicament>();
}