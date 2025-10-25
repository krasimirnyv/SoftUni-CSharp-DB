using Microsoft.EntityFrameworkCore;

namespace P02_FootballBetting.Data;

using Models;
using static Commons.ApplicationConstants;

public class FootballBettingContext : DbContext
{
    public FootballBettingContext()
    {
        
    }

    public FootballBettingContext(DbContextOptions<FootballBettingContext> options)
        : base(options)
    {

    }

    public virtual DbSet<Team> Teams { get; set; } = null!;
    
    public virtual DbSet<Color> Colors { get; set; } = null!;
    
    public virtual DbSet<Town> Towns { get; set; } = null!;
    
    public virtual DbSet<Country> Countries { get; set; } = null!;
    
    public virtual DbSet<Player> Players { get; set; } = null!;
    
    public virtual DbSet<Position> Positions { get; set; } = null!;
    
    public virtual DbSet<PlayerStatistic> PlayersStatistics { get; set; } = null!;
    
    public virtual DbSet<Game> Games { get; set; } = null!;
    
    public virtual DbSet<Bet> Bets { get; set; } = null!;
    
    public virtual DbSet<User> Users { get; set; } = null!;
    
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }
        
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<PlayerStatistic>(e =>
        {
            e.HasKey(ps => new { ps.PlayerId, ps.GameId });
        });

        builder.Entity<Team>(e =>
        {
            e
                .HasOne(t => t.PrimaryKitColor)
                .WithMany(c => c.PrimaryKitTeams)
                .HasForeignKey(t => t.PrimaryKitColorId)
                .OnDelete(DeleteBehavior.Restrict);

            e
                .HasOne(t => t.SecondaryKitColor)
                .WithMany(c => c.SecondaryKitTeams)
                .HasForeignKey(t => t.SecondaryKitColorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Game>(e =>
        {
            e
                .HasOne(g => g.HomeTeam)
                .WithMany(t => t.HomeGames)
                .HasForeignKey(g => g.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            e
                .HasOne(g => g.AwayTeam)
                .WithMany(t => t.AwayGames)
                .HasForeignKey(g => g.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Player>(e =>
        {
            e
                .HasOne(p => p.Town)
                .WithMany(t => t.Players)
                .HasForeignKey(p => p.TownId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        base.OnModelCreating(builder);
    }
}