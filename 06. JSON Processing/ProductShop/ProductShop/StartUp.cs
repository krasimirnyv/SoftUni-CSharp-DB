using System.ComponentModel.DataAnnotations;
using System.Globalization;

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ProductShop
{
    using Data;
    using DTOs.Import;
    using Models;

    public class StartUp
    {
        public static void Main()
        {
            using ProductShopContext dbContext = new ProductShopContext();
            
            // dbContext.Database.EnsureDeleted();
            // dbContext.Database.EnsureCreated();
            // Console.WriteLine("Successfully created the Shop DB");
            
            string jsonFileDirPath = Path
                .Combine(Directory.GetCurrentDirectory(), "../../../Datasets/");
            
            // Problem 01
            string jsonFileName = "users.json";
            string jsonFileTest = File.ReadAllText(jsonFileDirPath + jsonFileName);
            
            string result = ImportUsers(dbContext, jsonFileTest);
            Console.WriteLine(result);
            
            // Problem 02
            jsonFileName = "products.json";
            jsonFileTest = File.ReadAllText(jsonFileDirPath + jsonFileName);
            
            result = ImportProducts(dbContext, jsonFileTest);
            Console.WriteLine(result);
            
            // Problem 03
            jsonFileName = "categories.json";
            jsonFileTest = File.ReadAllText(jsonFileDirPath + jsonFileName);
            
            result = ImportCategories(dbContext, jsonFileTest);
            Console.WriteLine(result);
            
            // Problem 04
            jsonFileName = "categories.json";
            jsonFileTest = File.ReadAllText(jsonFileDirPath + jsonFileName);
            
            result = ImportCategoryProducts(dbContext, jsonFileTest);
            Console.WriteLine(result);
            
            // Problem 05
            result = GetProductsInRange(dbContext);
            Console.WriteLine(result);
            
            // Problem 06
            result = GetSoldProducts(dbContext);
            Console.WriteLine(result);
            
            // Problem 07
            result = GetCategoriesByProductsCount(dbContext);
            Console.WriteLine(result);
            
            // Problem 08
            result = GetUsersWithProducts(dbContext);
            Console.WriteLine(result);
        }

        // 01. Import Users
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            ICollection<User> usersToImport = new List<User>();

            IEnumerable<ImportUserDto>? userDtos = JsonConvert
                .DeserializeObject<ImportUserDto[]>(inputJson);

            if (userDtos == null)
                return string.Empty;

            foreach (ImportUserDto userDto in userDtos)
            {
                if(!IsValid(userDto))
                    continue;
                
                bool isUserAgeValid = int
                    .TryParse(userDto.Age, out int userAge);
                
                if (!isUserAgeValid && userDto.Age != null) 
                    continue;

                User user = new User()
                {
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Age = userDto.Age == null ? null : userAge
                };
                
                usersToImport.Add(user);
            }
            
            context.Users.AddRange(usersToImport);
            context.SaveChanges();
            
            return $"Successfully imported {usersToImport.Count}";
        }

        // 02. Import Products
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            ICollection<Product> productsToImport = new List<Product>();

            IEnumerable<ImportProductDto>? productDtos = JsonConvert
                .DeserializeObject<ImportProductDto[]>(inputJson);
            
            if (productDtos == null)
                return string.Empty;

            foreach (ImportProductDto productDto in productDtos)
            {
                if (!IsValid(productDto))
                    continue;
                
                bool isValidPriceValue = decimal
                    .TryParse(productDto.Price, NumberStyles.Any,
                        CultureInfo.InvariantCulture, out decimal productPrice);
                
                bool isValidSellerId = int
                    .TryParse(productDto.SellerId, out int productSellerId);
                
                bool isValidBuyerId = int
                    .TryParse(productDto.BuyerId, out int productBuyerId);
                
                if (!isValidPriceValue || !isValidSellerId || 
                    (!isValidBuyerId && productDto.BuyerId != null)) 
                    continue;

                Product product = new Product()
                {
                    Name = productDto.Name,
                    Price = productPrice,
                    SellerId = productSellerId,
                    BuyerId = productDto.BuyerId == null ? null : productBuyerId
                };
                
                productsToImport.Add(product);
            }
            
            context.Products.AddRange(productsToImport);
            context.SaveChanges();
            
            return $"Successfully imported {productsToImport.Count}";
        }

        // 03. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            ICollection<Category> categoriesToImport = new List<Category>();

            IEnumerable<ImportCategoryDto>? categoryDtos = JsonConvert
                .DeserializeObject<ImportCategoryDto[]>(inputJson);

            foreach (ImportCategoryDto categoryDto in categoryDtos)
            {
                if(!IsValid(categoryDto))
                    continue;

                Category category = new Category()
                {
                    Name = categoryDto.Name
                };
                
                categoriesToImport.Add(category);
            }
            
            context.Categories.AddRange(categoriesToImport);
            context.SaveChanges();
            
            return $"Successfully imported {categoriesToImport.Count}";
        }
        
        // 04. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            ICollection<CategoryProduct> categoriesProductsToImport = new List<CategoryProduct>();
            
            IEnumerable<ImportCategoryProductDto>? categoryProductDtos = JsonConvert
                .DeserializeObject<ImportCategoryProductDto[]>(inputJson);
            
            if (categoryProductDtos == null)
                return string.Empty;
            
            HashSet<int> existingCategoryIds = context
                .Categories
                .AsNoTracking()
                .Select(c => c.Id)
                .ToHashSet();

            HashSet<int> existingProductIds = context
                .Products
                .AsNoTracking()
                .Select(p => p.Id)
                .ToHashSet();
            
            foreach (ImportCategoryProductDto categoryProductDto in categoryProductDtos)
            {
                if (!IsValid(categoryProductDto))
                    continue;
                
                bool isCategoryIdValid = int
                    .TryParse(categoryProductDto.CategoryId, out int categoryId);
                
                bool isProductIdValid = int
                    .TryParse(categoryProductDto.ProductId, out int productId);
                
                if (!isCategoryIdValid || !isProductIdValid)
                    continue;

                if (!existingCategoryIds.Contains(categoryId) || 
                    !existingProductIds.Contains(productId))
                    continue;
                
                CategoryProduct categoryProduct = new CategoryProduct()
                {
                    CategoryId = categoryId,
                    ProductId = productId
                };
                
                categoriesProductsToImport.Add(categoryProduct);
            }
            
            context.CategoriesProducts.AddRange(categoriesProductsToImport);
            context.SaveChanges();
                
            return $"Successfully imported {categoriesProductsToImport.Count}";
        }
        
        // 05. Export Products in Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context
                .Products
                .Include(p => p.Seller)
                .AsNoTracking()
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new
                {
                    p.Name,
                    p.Price,
                    p.Seller.FirstName,
                    p.Seller.LastName
                })
                .ToArray();

            var productsToExport = products
                .Select(p => new
                {
                    name = p.Name,
                    price = p.Price,
                    seller = $"{p.FirstName} {p.LastName}"
                })
                .ToArray();

            string jsonResult = JsonConvert
                .SerializeObject(productsToExport, Formatting.Indented);

            return jsonResult;
        }
        
        // 06. Export Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            var soldProducts = context
                .Users
                .Include(u => u.ProductsSold)
                .AsNoTracking()
                .Where(u => u.ProductsSold.Count >= 1 &&
                            u.ProductsSold.Any(p => p.BuyerId != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    Products = u.ProductsSold
                        .Where(p => p.BuyerId != null)
                        .Select(p => new
                        {
                            p.Name,
                            p.Price,
                            p.Buyer.FirstName,
                            p.Buyer.LastName
                        })
                })
                .ToArray();

            var productsToExport = soldProducts
                .Select(sp => new
                {
                    firstName = sp.FirstName,
                    lastName = sp.LastName,
                    soldProducts = sp.Products
                        .Select(p => new
                        {
                            name = p.Name,
                            price = p.Price,
                            buyerFirstName = p.FirstName,
                            buyerLastName = p.LastName
                        })
                })
                .ToArray();

            string jsonResult = JsonConvert
                .SerializeObject(productsToExport, Formatting.Indented);

            return jsonResult;
        }
        
        // 07. Export Categories By Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context
                .Categories
                .Include(c => c.CategoriesProducts)
                .ThenInclude(cp => cp.Product)
                .AsNoTracking()
                .Select(c => new
                {
                    c.Name,
                    ProductsCount = c
                        .CategoriesProducts.Count,
                    AvgPrice = c.CategoriesProducts
                        .Average(cp => cp.Product.Price),
                    TotalPrice = c.CategoriesProducts
                        .Sum(cp => cp.Product.Price)
                })
                .OrderByDescending(c => c.ProductsCount)
                .ToArray();

            var categoriesToExport = categories
                .Select(c => new
                {
                    category = c.Name,
                    productsCount = c.ProductsCount,
                    averagePrice = c.AvgPrice.ToString("F2"),
                    totalRevenue = c.TotalPrice.ToString("F2")
                });

            string jsonResult = JsonConvert
                .SerializeObject(categoriesToExport, Formatting.Indented);

            return jsonResult;
        }
        
        // 08. Export Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context
                .Users
                .Include(u => u.ProductsSold)
                .AsNoTracking()
                .Where(u => u.ProductsSold
                    .Any(ps => ps.Buyer != null))
                .OrderByDescending(u => u.ProductsSold
                    .Count(ps => ps.Buyer != null))
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    age = u.Age,
                    soldProducts = new
                    {
                        count = u.ProductsSold.Count(ps => ps.Buyer != null),
                        products = u.ProductsSold
                            .Where(ps => ps.Buyer != null)
                            .Select(p => new
                            {
                                name = p.Name,
                                price = p.Price
                            })
                    }
                })
                .ToArray();

            var usersToExport = new
            {
                usersCount = users.Length,
                users
            };
            
            string jsonResult = JsonConvert
                .SerializeObject(usersToExport, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

            return jsonResult;
        }

        private static bool IsValid(object obj)
        {
            // These two variables are required by the Validator.TryValidateObject method
            // We will not use them for now...
            // We are just using the Validation result (true or false)
            
            ValidationContext validationContext = new ValidationContext(obj);
            ICollection<ValidationResult> validationResults = new List<ValidationResult>();
            
            return Validator
                .TryValidateObject(obj, validationContext, validationResults, true);
        }
    }
}