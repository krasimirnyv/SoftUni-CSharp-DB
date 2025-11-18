using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesApp.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Watchlist
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Movie))]
        public int MovieId { get; set; }

        public Movie Movie { get; set; } = null!;
    }
}
