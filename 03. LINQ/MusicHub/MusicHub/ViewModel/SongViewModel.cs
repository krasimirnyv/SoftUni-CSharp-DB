namespace MusicHub.ViewModel;

public class SongViewModel
{
    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public TimeSpan Duration { get; set; }

    public string WriterName { get; set; } = null!;

    public string? AlbumProducerName { get; set; }
    
    public IEnumerable<PerformerViewModel> PerformersNames { get; set; }
        = new List<PerformerViewModel>();
}