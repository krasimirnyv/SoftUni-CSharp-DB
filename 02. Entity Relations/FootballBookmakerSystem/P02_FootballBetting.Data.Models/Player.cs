using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P02_FootballBetting.Data.Models;

using static Commons.EntityValidation.Player;

public class Player
{
    [Key]
    public int PlayerId { get; set; }
    
    [Required]
    [MaxLength(NameMaxLength)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(SquadNumberMaxLength)]
    public string SquadNumber { get; set; } = null!;
    
    public bool IsInjured { get; set; }

    [ForeignKey(nameof(Position))]
    public int PositionId { get; set; }

    public virtual Position Position { get; set; } = null!;
    
    [ForeignKey(nameof(Team))]
    public int TeamId { get; set; }
    public virtual Team Team { get; set; } = null!;

    [ForeignKey(nameof(Town))]
    public int TownId { get; set; }
    public virtual Town Town { get; set; } = null!;
    
    public virtual ICollection<PlayerStatistic> PlayersStatistics { get; set; }
        = new HashSet<PlayerStatistic>();
}