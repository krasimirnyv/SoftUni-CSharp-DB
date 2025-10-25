using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace P01_HospitalDatabase.Data.Models;

using static Commons.EntityValidation.Diagnose;

public class Diagnose
{
    [Key]
    public int DiagnoseId { get; set; }

    [Required]
    [Unicode]
    [MaxLength(NameMaxLength)]
    public string Name { get; set; } = null!;
    
    [Unicode]
    [MaxLength(CommentsMaxLength)]
    public string? Comments { get; set; }
    
    [ForeignKey(nameof(Patient))]
    public int PatientId { get; set; }
    public virtual Patient Patient { get; set; } = null!;
}