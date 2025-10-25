using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace P01_StudentSystem.Data.Models;

using static Commons.EntityValidations.Student;

public class Student
{
    [Key]
    public int StudentId { get; set; }
    
    [Required] 
    [Unicode]
    [MaxLength(NameMaxLength)]
    public string Name { get; set; } = null!;

    [Unicode(PhoneNumberUnicode)]
    [MaxLength(PhoneNumberMaxLength)]
    public string? PhoneNumber { get; set; }
    
    public DateTime RegisteredOn { get; set; }

    public DateTime? Birthday { get; set; }


    public virtual ICollection<StudentCourse> StudentsCourses { get; set; }
        = new HashSet<StudentCourse>();
    
    public virtual ICollection<Homework> Homeworks { get; set; }
        = new HashSet<Homework>();
}