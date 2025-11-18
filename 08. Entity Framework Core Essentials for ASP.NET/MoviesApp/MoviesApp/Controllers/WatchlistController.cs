namespace MoviesApp.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    
    using Services.Interfaces;
    using ViewModels.Movies;
    
    public class WatchlistController : Controller
    {
        private readonly IMoviesService moviesService;
        private readonly IWatchlistService watchlistService;

        public WatchlistController(IMoviesService moviesService, IWatchlistService watchlistService)
        {
            this.moviesService = moviesService;
            this.watchlistService = watchlistService;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<AllMoviesIndexViewModel> moviesViewModel = await this.watchlistService
                .GetAllMoviesInWatchlistAsync();

            return View(moviesViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int id)
        {
            bool movieExists = await this.moviesService.ExistAsync(id);
            if (!movieExists)
                return NotFound();

            bool movieAddedToWatchlist = await this.watchlistService.AddMovieToWatchlistAsync(id);
            if (movieAddedToWatchlist)
                return RedirectToAction("Index", "Watchlist");

            return RedirectToAction("Index", "Movies");
        }

        public async Task<IActionResult> Details(int id)
        {
            MovieDetailsViewModel? viewModel = await this.moviesService.GetMovieDetailsByIdAsync(id);
            
            if (viewModel == null)
                return NotFound();

            return View(viewModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            await this.watchlistService.RemoveAsync(id);
            
            return RedirectToAction(nameof(Index));
        }
    }
}
