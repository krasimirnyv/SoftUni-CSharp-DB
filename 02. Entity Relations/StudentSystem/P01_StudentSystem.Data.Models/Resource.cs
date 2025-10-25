using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace P01_StudentSystem.Data.Models;

using static Commons.EntityValidations.Resource;

public class Resource
{
    [Key]
    public int ResourceId { get; set; }
    
    [Required]
    [Unicode]
    [MaxLength(NameMaxLength)]
    public string Name { get; set; } = null!;

    [Required]
    [Unicode(UrlUnicode)]
    [MaxLength(UrlMaxLength)]
    public string Url { get; set; } = null!;

    public ResourceType ResourceType { get; set; }

    [ForeignKey(nameof(Course))]
    public int CourseId { get; set; }

    public virtual Course Course { get; set; } = null!;
}