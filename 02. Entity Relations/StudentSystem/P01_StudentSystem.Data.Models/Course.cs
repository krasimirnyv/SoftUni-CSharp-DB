using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace P01_StudentSystem.Data.Models;

using static Commons.EntityValidations.Course;

public class Course
{
    [Key]
    public int CourseId { get; set; }

    [Required]
    [MaxLength(NameMaxLength)]
    public string Name { get; set; } = null!;

    [Unicode]
    [MaxLength(DescriptionMaxLength)]
    public string? Description { get; set; }

    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }

    [Column(TypeName = AmountColumnType)]
    public decimal Price { get; set; }
    
    
    public virtual ICollection<StudentCourse> StudentsCourses { get; set; }
        = new HashSet<StudentCourse>();

    public virtual ICollection<Resource> Resources { get; set; }
        = new HashSet<Resource>();

    public virtual ICollection<Homework> Homeworks { get; set; }
        = new HashSet<Homework>();
}