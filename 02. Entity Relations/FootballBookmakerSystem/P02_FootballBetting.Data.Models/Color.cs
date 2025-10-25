using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P02_FootballBetting.Data.Models;

using static Commons.EntityValidation.Color;

public class Color
{
    [Key]
    public int ColorId { get; set; }

    [Required] 
    [MaxLength(NameMaxLength)] 
    public string Name { get; set; } = null!;

    [InverseProperty(nameof(Team.PrimaryKitColor))]
    public virtual ICollection<Team> PrimaryKitTeams { get; set; }
        = new HashSet<Team>();
    
    [InverseProperty(nameof(Team.SecondaryKitColor))]
    public virtual ICollection<Team> SecondaryKitTeams { get; set; }
        = new HashSet<Team>();
}