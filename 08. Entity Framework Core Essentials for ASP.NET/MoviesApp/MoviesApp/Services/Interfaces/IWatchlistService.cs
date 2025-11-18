namespace MoviesApp.Services.Interfaces
{
    using ViewModels.Movies;
    
    public interface IWatchlistService
    {
        Task<bool> AddMovieToWatchlistAsync(int movieId);

        Task<IEnumerable<AllMoviesIndexViewModel>> GetAllMoviesInWatchlistAsync();
        
        Task RemoveAsync(int movieId);
        
        Task<bool> MovieExistsInWatchlistAsync(int movieId);
    }
}
