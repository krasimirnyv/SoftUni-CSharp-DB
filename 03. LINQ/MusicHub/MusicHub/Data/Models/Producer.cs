using System.ComponentModel.DataAnnotations;

namespace MusicHub.Data.Models;

using static Common.EntityValidation.Producer;

public class Producer
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(NameMaxLength)]
    public string Name { get; set; } = null!;

    [MaxLength(PseudonymMaxLength)]
    public string? Pseudonym { get; set; }

    [MaxLength(PhoneNumberMaxLength)]
    public string? PhoneNumber { get; set; }

    public virtual ICollection<Album> Albums { get; set; }
        = new HashSet<Album>();
}