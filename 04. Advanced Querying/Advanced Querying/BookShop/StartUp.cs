using System.Text;
using BookShop.Models;
using Microsoft.EntityFrameworkCore;

namespace BookShop
{
    using Data;
    using Initializer;
    using Models.Enums;
    
    public class StartUp
    {
        // Compiled query for Problem 06. Book Titles by Category
        private static readonly Func<BookShopContext, string[], IEnumerable<string>> _compiledBookTitlesQuery =
            EF.CompileQuery((BookShopContext context, string[] categories) =>
                context
                    .Books
                    .AsNoTracking()
                    .Where(b => b.BookCategories
                        .Select(bc => bc.Category)
                        .Any(c => categories.Contains(c.Name.ToLower())))
                    .OrderBy(b => b.Title)
                    .Select(b => b.Title));
        
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);

            int input = int.Parse(Console.ReadLine());
            int result = CountBooks(db, input);
            Console.WriteLine(result);
        }

        // 02. Age Restriction
        // Shows that Enumerations are optimized for search in DB
        // Used as a parameterized query -> EF Cache is working properly
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            bool isCommandValid = Enum.TryParse(command, true, out AgeRestriction ageRestriction);
            if (!isCommandValid)
            {
                return string.Empty;
            }
               
            string[] bookTitles = context
                .Books
                .AsNoTracking()
                .Where(b => b.AgeRestriction == ageRestriction)
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bookTitles.Length - 1; i++)
            {
                sb.AppendLine(bookTitles[i]);
            }
            
            sb.Append(bookTitles[^1]);

            return sb.ToString();
        }

        // 03. Golden Books
        public static string GetGoldenBooks(BookShopContext context)
        {
            string[] bookTitles = context
                .Books
                .AsNoTracking()
                .Where(b => b.EditionType == EditionType.Gold &&
                            b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bookTitles.Length - 1; i++)
                sb.AppendLine(bookTitles[i]);

            sb.Append(bookTitles[^1]);

            return sb.ToString();
        }

        // 04. Books by Price
        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context
                .Books
                .AsNoTracking()
                .Where(b => b.Price > 40)
                .OrderByDescending(b => b.Price)
                .Select(b => new
                {
                    b.Title,
                    b.Price
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:F2}");
            }

            return sb.ToString().TrimEnd();
        }
        
        // 05. Not Released In
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            string[] bookTitles = context
                .Books
                .AsNoTracking()
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();
            
            StringBuilder sb = new StringBuilder();
            
            for (int i = 0; i < bookTitles.Length - 1; i++)
                sb.AppendLine(bookTitles[i]);
            
            sb.Append(bookTitles[^1]);
            
            return sb.ToString();
        }
        
        // 06. Book Titles by Category
        // Shows that searching inside collections is NOT optimized for DB
        // Used as non-parameterized query -> EF Cache is NOT working properly / 300 ms /
        // Note: Fixed as of EF 9.0
        // Alt: Use pre-compiled query -> / 70 ms /
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLowerInvariant())
                .ToArray();

            string[] bookTitles = context
                .Books
                .AsNoTracking()
                .Where(b => b.BookCategories
                    .Select(bc => bc.Category)
                    .Any(c => categories.Contains(c.Name.ToLower())))
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bookTitles.Length - 1; i++)
                sb.AppendLine(bookTitles[i]);
            
            sb.Append(bookTitles[^1]);

            return sb.ToString();
        }
        
        public static string GetBooksByCategory_Compiled(BookShopContext context, string input)
        {
            string[] categories = input
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLowerInvariant())
                .ToArray();

            string[] bookTitles = _compiledBookTitlesQuery(context, categories)
                .ToArray();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bookTitles.Length - 1; i++)
                sb.AppendLine(bookTitles[i]);
            
            sb.Append(bookTitles[^1]);

            return sb.ToString();
        }
        
        // 07. Released Before Date
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            DateTime dateTime = DateTime.ParseExact(date, "dd-MM-yyyy", null);

            var books = context
                .Books
                .AsNoTracking()
                .Where(b => b.ReleaseDate < dateTime)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:F2}");
            }
            
            return sb.ToString().TrimEnd();
        }
        
        // 08. Author Search
        // Shows that .StartsWith() and .EndsWith() are optimized for use in DB
        // EF Core generates parameterized queries for these methods
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context
                .Authors
                .AsNoTracking()
                .Where(a => a.FirstName.EndsWith(input))
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .Select(a => new
                {
                    a.FirstName,
                    a.LastName
                })
                .ToArray();
            
            StringBuilder sb = new StringBuilder();
            foreach (var author in authors)
            {
                sb.AppendLine($"{author.FirstName} {author.LastName}");
            }

            return sb.ToString().TrimEnd();
        }
        
        // 09. Book Search
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            string[] bookTitles = context
                .Books
                .AsNoTracking()
                .Where(b => b.Title
                    .ToLower()
                    .Contains(input.ToLower()))
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bookTitles.Length - 1; i++)
                sb.AppendLine(bookTitles[i]);
            
            sb.Append(bookTitles[^1]);
            
            return sb.ToString();
        }
        
        // 10. Book Search by Author
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context
                .Books
                .Include(b => b.Author)
                .AsNoTracking()
                .Where(b => b
                    .Author
                    .LastName
                    .ToLower()
                    .StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .Select(b => new
                {
                    b.Title,
                    b.Author.FirstName,
                    b.Author.LastName
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (var bookByAuthor in books)
            {
                sb.AppendLine($"{bookByAuthor.Title} ({bookByAuthor.FirstName} {bookByAuthor.LastName})");
            }
            
            return sb.ToString().TrimEnd();
        }
        
        // 11. Count Books
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            int booksLength = context
                .Books
                .AsNoTracking()
                .Where(b => b.Title.Length > lengthCheck)
                .Select(b => b.Title)
                .ToArray()
                .Length;

            return booksLength;
        }
        
        // 12. Total Book Copies
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            // TODO: Implement the method 
            throw new NotImplementedException();
        }
        
        // TODO: Finish the homework form Problem 12 to 16
    }
}


