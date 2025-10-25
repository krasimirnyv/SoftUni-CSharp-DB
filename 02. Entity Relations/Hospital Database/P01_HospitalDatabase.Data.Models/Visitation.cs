using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace P01_HospitalDatabase.Data.Models;

using static Commons.EntityValidation.Visitation;


public class Visitation
{
    [Key]
    public int VisitationId { get; set; }
    
    public DateTime Date { get; set; }
    
    [Unicode]
    [MaxLength(CommentsMaxLength)]
    public string? Comments { get; set; }
    
    [ForeignKey(nameof(Patient))] 
    public int PatientId { get; set; }
    public virtual Patient Patient { get; set; } = null!;
    
    [ForeignKey(nameof(Doctor))]
    public int DoctorId { get; set; }
    public virtual Doctor Doctor { get; set; } = null!;
}