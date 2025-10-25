using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace P01_StudentSystem.Data.Models;

using static Commons.EntityValidations.Homework;

public class Homework
{
    [Key]
    public int HomeworkId { get; set; }

    [Unicode(ContentUnicode)]
    [MaxLength(ContentMaxLength)]
    public string Content { get; set; }
    
    public ContentType ContentType { get; set; }

    public DateTime SubmissionTime { get; set; }
    
    [ForeignKey(nameof(Student))]
    public int StudentId { get; set; }
    
    public virtual Student Student { get; set; } = null!;
    
    [ForeignKey(nameof(Course))]
    public int CourseId { get; set; }
    
    public virtual Course Course { get; set; } = null!;
}