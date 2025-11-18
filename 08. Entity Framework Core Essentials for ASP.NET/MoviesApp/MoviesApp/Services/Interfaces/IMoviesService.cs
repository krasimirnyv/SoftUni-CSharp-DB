namespace MoviesApp.Services.Interfaces
{
    using ViewModels.Movies;
    
    public interface IMoviesService
    {
        Task CreateAsync(AddMovieFormModel inputModel);
        
        Task<bool> DeleteAsync(int id);

        Task<bool> ExistAsync(int id);

        Task<IEnumerable<AllMoviesIndexViewModel>> GetAllMoviesForListingAsync();

        Task<MovieDetailsViewModel?> GetMovieDetailsByIdAsync(int id);

        Task<AllMoviesIndexViewModel?> GetMoviePrepareDeleteViewModelByIdAsync(int id);
    }
}
