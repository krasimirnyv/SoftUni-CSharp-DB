using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ProductShop
{
    using Data;
    using Models;
    using Utilities;
    
    using DTOs.Import;
    using DTOs.Export;

    public class StartUp
    {
        public static void Main()
        {
            using ProductShopContext dbContext = new ProductShopContext();

            ResetAndSeedDatabase(dbContext);
            PrintResult(dbContext);
        }

        // 01. Import Users
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            ICollection<User> usersToImport = new List<User>();

            ImportUserDto[]? userDtos = XmlSerializerWrapper
                .Deserialize<ImportUserDto[]>(inputXml, "Users");
            
            if (userDtos == null)
                return string.Empty;

            foreach (ImportUserDto userDto in userDtos)
            {
                if(!IsValid(userDto))
                    continue;
                
                bool isAgeValid = TryParseNullable(userDto.Age, out int? age);
                if (!isAgeValid)
                    continue;

                User user = new User
                {
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Age = age,
                };
                
                usersToImport.Add(user);
            }
            
            context.Users.AddRange(usersToImport);
            context.SaveChanges();
            
            return $"Successfully imported {usersToImport.Count}";
        }

        // 02. Import Products
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            ICollection<Product> productsToImport = new List<Product>();

            ImportProductDto[]? productDtos = XmlSerializerWrapper
                .Deserialize<ImportProductDto[]>(inputXml, "Products");
            
            if (productDtos == null)
                return string.Empty;

            int[] existingUserIds = context
                .Users
                .AsNoTracking()
                .Select(u => u.Id)
                .ToArray();
            
            foreach (ImportProductDto productDto in productDtos)
            {
                if (!IsValid(productDto))
                    continue;
                
                bool isValidPriceValue = decimal
                    .TryParse(productDto.Price, NumberStyles.Any,
                        CultureInfo.InvariantCulture, out decimal price);
                
                bool isValidSellerId = int
                    .TryParse(productDto.SellerId, out int sellerId);
                
                bool isValidBuyerId = TryParseNullable(productDto.BuyerId, out int? buyerIdNullable);
                
                if (!isValidPriceValue || !isValidSellerId || !isValidBuyerId) 
                    continue;

                if (buyerIdNullable != null)
                {
                    int buyerId = buyerIdNullable.Value;
                    
                    if(!existingUserIds.Contains(sellerId) || !existingUserIds.Contains(buyerId) || sellerId == buyerId)
                        continue;
                }

                Product product = new Product()
                {
                    Name = productDto.Name,
                    Price = price,
                    SellerId = sellerId,
                    BuyerId = buyerIdNullable
                };
                
                productsToImport.Add(product);
            }
            
            context.Products.AddRange(productsToImport);
            context.SaveChanges();
            
            return $"Successfully imported {productsToImport.Count}";
        }

        // 03. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            ICollection<Category> categoriesToImport = new List<Category>();

            ImportCategoryDto[]? categoryDtos = XmlSerializerWrapper
                .Deserialize<ImportCategoryDto[]>(inputXml, "Categories");

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
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            ICollection<CategoryProduct> categoriesProductsToImport = new List<CategoryProduct>();
            
            ImportCategoryProductDto[]? categoryProductDtos = XmlSerializerWrapper
                .Deserialize<ImportCategoryProductDto[]>(inputXml, "CategoryProducts");
            
            if (categoryProductDtos == null)
                return string.Empty;
            
            int[] existingCategoryIds = context
                .Categories
                .AsNoTracking()
                .Select(c => c.Id)
                .ToArray();

            int[] existingProductIds = context
                .Products
                .AsNoTracking()
                .Select(p => p.Id)
                .ToArray();
            
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
            
            context.CategoryProducts.AddRange(categoriesProductsToImport);
            context.SaveChanges();
                
            return $"Successfully imported {categoriesProductsToImport.Count}";
        }
        
        // 05. Export Products in Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            ExportProductDto[] products = context
                .Products
                .Include(p => p.Buyer)
                .AsNoTracking()
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new ExportProductDto()
                {
                    Name = p.Name,
                    Price = p.Price,
                    Buyer = p.Buyer != null ? $"{p.Buyer.FirstName} {p.Buyer.LastName}" : " "
                })
                .Take(10)
                .ToArray();
            
            string result = XmlSerializerWrapper
                .Serialize(products, "Products");

            return result;
        }
        
        // 06. Export Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            ExportUserDto[] usersSoldProducts = context
                .Users
                .Include(u => u.ProductsSold)
                .AsNoTracking()
                .Where(u => u.ProductsSold.Count > 0)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new ExportUserDto()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold
                        .Select(ps => new ExportSoldProductDto()
                        {
                            Name = ps.Name,
                            Price = ps.Price
                        })
                        .ToArray()
                })
                .Take(5)
                .ToArray();

            string result = XmlSerializerWrapper
                .Serialize(usersSoldProducts, "Users");
            
            return result;
        }
        
        // 07. Export Categories By Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            ExportCategoryDto[] categories = context
                .Categories
                .Include(c => c.CategoriesProducts)
                .ThenInclude(cp => cp.Product)
                .AsNoTracking()
                .Select(c => new ExportCategoryDto()
                {
                    Name = c.Name,
                    Count = c.CategoriesProducts
                        .Select(cp => cp.Product)
                        .Count(),
                    AveragePrice = c.CategoriesProducts
                        .Select(cp => cp.Product.Price)
                        .Average(),
                    TotalRevenue = c.CategoriesProducts
                        .Select(cp => cp.Product.Price)
                        .Sum()
                })
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalRevenue)
                .ToArray();
            
            string result = XmlSerializerWrapper
                .Serialize(categories, "Categories");
            
            return result;
        }
        
        // 08. Export Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            ExportUsersCountDto rootDto = new ExportUsersCountDto()
            {
                TotalUsersCount = context
                    .Users
                    .Include(u => u.ProductsSold)
                    .AsNoTracking()
                    .Count(u => u.ProductsSold.Any()),
                Users = context
                    .Users
                    .Include(u => u.ProductsSold)
                    .AsNoTracking()
                    .Where(u => u.ProductsSold.Any())
                    .Select(u => new ExportUserWithSoldProductsDto()
                    {
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Age = u.Age,
                        SoldProducts = new ExportUserSoldProductsDto()
                        {
                            Count = u.ProductsSold.Count,
                            Products = u.ProductsSold
                                .OrderByDescending(p => p.Price)
                                .Select(p => new ExportSoldProductDto()
                                {
                                    Name = p.Name,
                                    Price = p.Price
                                })
                                .ToArray()
                        }
                    })
                    .OrderByDescending(u => u.SoldProducts.Count)
                    .Take(10)
                    .ToArray()
            };

            string result = XmlSerializerWrapper
                .Serialize(rootDto, "Users");
            
            return result;
        }

        private static void ResetAndSeedDatabase(ProductShopContext dbContext)
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            Console.WriteLine("Successfully created the Shop DB");
            
            // Problem 01
            string xmlFileName = "users.xml";
            string xmlFileText = ReadXmlDatasetFileContents(xmlFileName);
            string result = ImportUsers(dbContext, xmlFileText);
            
            Console.WriteLine(result);
            
            // Problem 02
            xmlFileName = "products.xml";
            xmlFileText = ReadXmlDatasetFileContents(xmlFileName);
            result = ImportProducts(dbContext, xmlFileText);
            
            Console.WriteLine(result);
            
            // Problem 03
            xmlFileName = "categories.xml";
            xmlFileText = ReadXmlDatasetFileContents(xmlFileName);
            result = ImportCategories(dbContext, xmlFileText);
            
            Console.WriteLine(result);
            
            // Problem 04
            xmlFileName = "categories-products.xml";
            xmlFileText = ReadXmlDatasetFileContents(xmlFileName);
            result = ImportCategoryProducts(dbContext, xmlFileText);
            
            Console.WriteLine(result);
        }
        
        private static string ReadXmlDatasetFileContents(string fileName)
        {
            string xmlFileDirPath = Path
                .Combine(Directory.GetCurrentDirectory(), "../../../Datasets/");
            
            string xmlFileText = File
                .ReadAllText(xmlFileDirPath + fileName);

            return xmlFileText;
        }
        
        private static void PrintResult(ProductShopContext dbContext)
        {
            // Problem 05
            string fileName = "products-in-range.xml";
            string result = GetProductsInRange(dbContext);
            WriteSerializationResult(fileName, result);
            Console.WriteLine(result);
            
            // Problem 06
            fileName = "users-sold-products.xml";
            result = GetSoldProducts(dbContext);
            WriteSerializationResult(fileName, result);
            Console.WriteLine(result);
            
            // Problem 07
            fileName = "categories-by-products.xml";
            result = GetCategoriesByProductsCount(dbContext);
            WriteSerializationResult(fileName, result);
            Console.WriteLine(result);
            
            // Problem 08
            fileName = "users-and-products.xml";
            result = GetUsersWithProducts(dbContext);
            WriteSerializationResult(fileName, result);
            Console.WriteLine(result);
        }
        
        private static void WriteSerializationResult(string fileName, string text)
        {
            string xmlFileDirPath = Path
                .Combine(Directory.GetCurrentDirectory(), "../../../Results/");
            File
                .WriteAllText(xmlFileDirPath + fileName, text, Encoding.Unicode);
        }

        private static bool TryParseNullable<T>(string? input, out T? value)
            where T : struct
        {
            value = null;
            
            if (input == null)
                return true;

            var tryParseMethod = typeof(T)
                .GetMethod("TryParse", new[] { typeof(string), typeof(T).MakeByRefType() });
            
            if (tryParseMethod == null)
                throw new InvalidOperationException($"Type {typeof(T)} does not support TryParse(string, out {typeof(T)})");

            object?[] parameters = { input, null! };
                
            bool success = (bool)tryParseMethod.Invoke(null, parameters)!;
        
            if (!success)
                return false;
            
            value = (T?)parameters[1];
            return true;
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