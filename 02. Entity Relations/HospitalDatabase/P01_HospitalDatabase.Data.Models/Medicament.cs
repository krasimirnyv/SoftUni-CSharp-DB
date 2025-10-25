using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace P01_HospitalDatabase.Data.Models;

using static Commons.EntityValidation.Medicament;

public class Medicament
{
    [Key]
    public int MedicamentId { get; set; }

    [Required]
    [Unicode]
    [MaxLength(NameMaxLength)]
    public string Name { get; set; } = null!;
    
    public virtual ICollection<PatientMedicament> Prescriptions { get; set; }
        = new HashSet<PatientMedicament>();
}