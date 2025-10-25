using System.ComponentModel.DataAnnotations;

namespace P02_FootballBetting.Data.Models;

using static Commons.EntityValidation.Country;

public class Country
{
    [Key] 
    public int CountryId { get; set; }
    
    [Required]
    [MaxLength(NameMaxLength)]
    public string Name { get; set; } = null!;
    
    public virtual ICollection<Town> Towns { get; set; }
        = new HashSet<Town>();
}