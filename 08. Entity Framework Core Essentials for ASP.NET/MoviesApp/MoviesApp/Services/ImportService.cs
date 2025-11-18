namespace MoviesApp.Services
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    
    using static Common.EntityValidation;
    using Data;
    using DTOs.Json;
    using DTOs.Xml;
    using Interfaces;
    using Models;
    using Utilities;

    using Newtonsoft.Json;

    public class ImportService : IImportService
    {
        private readonly MoviesAppDbContext dbContext;
        
        public ImportService(MoviesAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        
        public async Task<int> ImportFromJsonAsync(string fileName)
        {
            string jsonFileContent = this.ReadDatasetFileContents(fileName);
            
            ICollection<Movie> moviesToImport = new List<Movie>();
            
            IEnumerable<ImportJsonMovieDto>? importedMovieDtos = JsonConvert
                .DeserializeObject<ImportJsonMovieDto[]>(jsonFileContent);

            if (importedMovieDtos == null)
                return 0;

            foreach (ImportJsonMovieDto movieDto in importedMovieDtos)
            {
                if(!this.IsValid(movieDto))
                    continue;
                
                bool isReleaseDateValid = DateOnly
                    .TryParseExact(movieDto.ReleaseDate, "yyyy-MM-dd", 
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly releaseDate);
                
                if (!isReleaseDateValid)
                    continue;

                if (this.dbContext.Movies.Any(m => 
                        m.Title == movieDto.Title && 
                        m.ReleaseDate == releaseDate))
                {
                    /* Based on assumption that (the Movie Title together with the Movie ReleaseDate) are unique for each movie */
                    /* This will prevent double import even if the application is restarted */
                    continue;
                }
                
                Movie newMovie = new Movie()
                {
                    Title = movieDto.Title,
                    Genre = movieDto.Genre,
                    ReleaseDate = releaseDate,
                    Director = movieDto.Director,
                    Duration = movieDto.Duration,
                    Description = movieDto.Description,
                    ImageUrl = movieDto.ImageUrl
                };
                
                moviesToImport.Add(newMovie);
            }
            
            await this.dbContext.Movies.AddRangeAsync(moviesToImport);
            await this.dbContext.SaveChangesAsync();

            return moviesToImport.Count;
        }

        public async Task<int> ImportFromXmlAsync(string fileName)
        {
            /* Using dynamic LINQ-to-XML using XDoc will be more suitable */

            const string xmlRootElement = "MoviesLibrary";
            string xmlFileContent = this.ReadDatasetFileContents(fileName);
            
            ICollection<Movie> moviesToImport = new List<Movie>();
            IEnumerable<ImportXmlGenreGroupDto>? importedGenreGroupDtos = XmlSerializerWrapper
                .Deserialize<ImportXmlGenreGroupDto[]>(xmlFileContent, xmlRootElement);
            
            if (importedGenreGroupDtos == null)
                return 0;

            foreach (ImportXmlGenreGroupDto genreGroupDto in importedGenreGroupDtos)
            {
                if (!this.IsValid(genreGroupDto))
                    continue;

                foreach (ImportXmlGenreGroupMovieDto movieDto in genreGroupDto.Movies)
                {
                    if (!this.IsValid(movieDto))
                        continue;
                    
                    bool isDurationValid = int.TryParse(movieDto.Duration, out int durationValue);
                    
                    if((!isDurationValid) || 
                       (durationValue < MovieDurationMinValue) ||
                       (durationValue > MovieDurationMaxValue))
                        continue;
                    
                    if (!this.ValidateMovieDetails(movieDto.Details, genreGroupDto.Name, out DateOnly releaseDate))
                        continue;
                    
                    if (this.dbContext.Movies.Any(m =>
                            m.Title == movieDto.Title &&
                            m.ReleaseDate == releaseDate))
                    {
                        continue;
                    }
                    
                    if((movieDto.Media != null) &&
                       (!this.IsValid(movieDto.Media)))
                        continue;
                    
                    /* Optionally add validation of the rating, it is intended to be used */

                    Movie newMovie = new Movie()
                    {
                        Title = movieDto.Title,
                        Genre = movieDto.Details.Genre,
                        ReleaseDate = releaseDate,
                        Director = movieDto.Details.Director,
                        Duration = durationValue,
                        Description = movieDto.Description,
                        ImageUrl = movieDto.Media?.ImageUrl
                    };
                    
                    moviesToImport.Add(newMovie);
                }
            }
            
            await this.dbContext.Movies.AddRangeAsync(moviesToImport);
            await this.dbContext.SaveChangesAsync();
            
            return moviesToImport.Count;
        }
        
        private bool ValidateMovieDetails(ImportXmlMovieDetailsDto xmlMovieDetails, string genreGroup, out DateOnly releaseDate)
        {
            bool result = this.IsValid(xmlMovieDetails);
            
            if(xmlMovieDetails.Genre != genreGroup)
                result = false;
            
            bool isReleaseDateValid = DateOnly
                .TryParseExact(xmlMovieDetails.Release.Date, "yyyy-MM-dd", 
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out releaseDate);
            
            if(!isReleaseDateValid)
                result = false;

            return result;
        }
        
        private string ReadDatasetFileContents(string fileName)
        {
            string fileDirPath = Path
                .Combine(Directory.GetCurrentDirectory(), "./Datasets/");
            
            string fileText = File
                .ReadAllText(fileDirPath + fileName);
            
            return fileText;
        }
        
        private bool IsValid(object obj)
        {
            ValidationContext validationContext = new ValidationContext(obj);
            ICollection<ValidationResult> validationResults
                = new List<ValidationResult>();

            return Validator
                .TryValidateObject(obj, validationContext, validationResults);
        }
    }
}
