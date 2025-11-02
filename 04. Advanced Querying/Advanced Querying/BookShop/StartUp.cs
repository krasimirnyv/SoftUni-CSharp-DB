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
            
            // string input by default for all problems
            string input = Console.ReadLine();
            // result is set here and used from now on for all problems
            string result = string.Empty;

            // Problem 02
            result = GetBooksByAgeRestriction(db, input);
            
            // Problem 03
            result = GetGoldenBooks(db);
            
            // Problem 04
            result = GetBooksByPrice(db);
            
            // Problem 05
            if (int.TryParse(input, out int year))
                result = GetBooksNotReleasedIn(db, year);
            else
                Console.WriteLine("Invalid year input. Input must be Integer!");
            
            // Problem 06.1 - No compiled
            result = GetBooksByCategory(db, input);
            
            // Problem 06.2 - Compiled
            result = GetBooksByCategory_Compiled(db, input);
            
            // Problem 07
            result = GetBooksReleasedBefore(db, input);
            
            // Problem 08
            result = GetAuthorNamesEndingIn(db, input);
            
            // Problem 09
            result = GetBookTitlesContaining(db, input);
            
            // Problem 10
            result = GetBooksByAuthor(db, input);
            
            // Problem 11
            if (int.TryParse(input, out int length))
                result = CountBooks(db, length).ToString(); // The method returns integer
            else
                Console.WriteLine("Invalid year input. Input must be Integer!");
            
            // Problem 12
            result = CountCopiesByAuthor(db);
            
            // Problem 13
            result = GetTotalProfitByCategory(db);
            
            // Problem 14
            result = GetMostRecentBooks(db);
            
            // Problem 15 - the three ways of bulk update
            IncreasePrices(db);
            IncreasePrices_ThirdParty_Bulk(db);
            IncreasePrices_Native_EF_8(db);
            
            // Problem 16 - the three ways of bulk delete
            result = RemoveBooks(db).ToString(); // The method returns integer
            result = RemoveBooks_ThirdParty_Bulk(db).ToString(); // The method returns integer
            result = RemoveBooks_Native_EF_8(db).ToString(); // The method returns integer
            
            // Printing the result from the selected method
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
        // Shows using of aggregation functions in DB queries is optimal
        // EF Core translates aggregation functions to SQL properly, but we should be careful
        // Keep in mind to use .AsSplitQuery() if required
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authorsCopies = context
                .Authors
                .AsNoTracking()
                .Select(a => new
                {
                    a.FirstName,
                    a.LastName,
                    Copies = a.Books
                        .Sum(b => b.Copies)
                })
                .OrderByDescending(a => a.Copies)
                .ToArray();
            
            StringBuilder sb = new StringBuilder();
            foreach (var author in authorsCopies)
            {
                sb.AppendLine($"{author.FirstName} {author.LastName} - {author.Copies}");
            }

            return sb.ToString().TrimEnd();
        }
        
        // 13. Profit by Category
        // Shows that aggregate functions with navigation properties are optimized for DB
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context
                .Categories
                .Include(c => c.CategoryBooks)
                .ThenInclude(cb => cb.Book)
                .AsNoTracking()
                .Select(c => new
                {
                    c.Name,
                    TotalProfit = c.CategoryBooks
                        .Select(cb => cb.Book)
                        .Sum(b => b.Price * b.Copies)
                })
                .OrderByDescending(c => c.TotalProfit)
                .ThenBy(c => c.Name)
                .AsSplitQuery()
                .ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (var profit in categories)
            {
                sb.AppendLine($"{profit.Name} ${profit.TotalProfit:F2}");
            }

            return sb.ToString().TrimEnd();
        }
        
        // 14. Most Recent Books
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context
                .Categories
                .Include(c => c.CategoryBooks)
                .ThenInclude(cb => cb.Book)
                .AsNoTracking()
                .Select(c => new
                {
                    CategoryName = c.Name,
                    Books = c.CategoryBooks
                        .Select(cb => cb.Book)
                        .OrderByDescending(b => b.ReleaseDate)
                        .Take(3)

                })
                .OrderBy(c => c.CategoryName)
                .AsSplitQuery()
                .ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (var categoryBooks in categories)
            {
                sb.AppendLine($"--{categoryBooks.CategoryName}");

                foreach (Book book in categoryBooks.Books)
                {
                    sb.AppendLine($"{book.Title} ({book.ReleaseDate!.Value.Year})");
                }
            }

            return sb.ToString().TrimEnd();
        }
        
        // 15. Increase Prices
        // Older versions of EF Core < 8.0 can't handle bulk update/delete
        // They are performing multiple atomic updates/deletes -> inefficient query
        public static void IncreasePrices(BookShopContext context)
        {
            IQueryable<Book> booksToUpdate = context
                .Books
                .Where(b => b.ReleaseDate != null &&
                            b.ReleaseDate.Value.Year < 2010);

            foreach (Book book in booksToUpdate)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }
        
        // For older EF Core versions < 8.0, we can use third party Z.EntityFramework.Plus.EFCore
        // NuGet packet for bulk operations, but it's paid
        public static void IncreasePrices_ThirdParty_Bulk(BookShopContext context)
        {
            context
                .Books
                .Where(b => b.ReleaseDate != null &&
                            b.ReleaseDate.Value.Year < 2010)
                .Update(b => new Book
                {
                    Price = b.Price + 5
                });
        }

        // EF Core 8.0+ has built-in support for bulk updates / delete
        public static void IncreasePrices_Native_EF_8(BookShopContext context)
        {
            context
                .Books
                .Where(b => b.ReleaseDate != null &&
                            b.ReleaseDate.Value.Year < 2010)
                .ExecuteUpdate(e => e.SetProperty(b => b.Price, b => b.Price + 5));
        }
        
        
        // 16. Remove Books
        // Older versions of EF Core < 8.0 can't handle bulk update/delete
        // They are performing multiple atomic updates/deletes -> inefficient query
        public static int RemoveBooks(BookShopContext context)
        {
            IQueryable<Book> booksToRemove = context
                .Books
                .Where(b => b.Copies < 4200);
            
            int count = booksToRemove.Count();
            
            context.RemoveRange(booksToRemove);
            context.SaveChanges();
            
            return count;
        }
        
        // For older EF Core versions < 8.0, we can use third party Z.EntityFramework.Plus.EFCore
        // NuGet packet for bulk operations, but it's paid
        public static int RemoveBooks_ThirdParty_Bulk(BookShopContext context)
        {
            var booksToRemove = context
                .Books
                .Where(b => b.Copies < 4200);
            
            int count = booksToRemove.Count();

            context.BulkDelete(booksToRemove);
            
            return count;
        }

        // EF Core 8.0+ has built-in support for bulk updates / delete
        public static int RemoveBooks_Native_EF_8(BookShopContext context)
        {
            int count = context
                .Books
                .Count(b => b.Copies < 4200);
            
            context
                .Books
                .Where(b => b.Copies < 4200)
                .ExecuteDelete();
            
            return count;
        }
    }
}


