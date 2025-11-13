using System.Text;
using System.Globalization;
using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

namespace CarDealer
{
    using Utilities;
    
    using Data;
    using Models;
    
    using DTOs.Import;
    using DTOs.Export;
    
    public class StartUp
    {
        public static void Main()
        {
            using CarDealerContext dbContext = new CarDealerContext();

            ResetAndSeedDatabase(dbContext);
            PrintResult(dbContext);
        }
        
        // 01. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            ICollection<Supplier> suppliers = new List<Supplier>();

            ImportSupplierDto[]? supplierDtos = XmlSerializerWrapper
                .Deserialize<ImportSupplierDto[]>(inputXml, "Suppliers");

            if (supplierDtos == null)
                return string.Empty;
            
            foreach (ImportSupplierDto supplierDto in supplierDtos)
            {
                if (!IsValid(supplierDto))
                    continue;
                
                bool isValidBooleanValue = bool
                    .TryParse(supplierDto.IsImporter, out bool isImporter);
                
                if (!isValidBooleanValue)
                    continue;

                Supplier newSupplier = new Supplier()
                {
                    Name = supplierDto.Name,
                    IsImporter = isImporter
                };
                
                suppliers.Add(newSupplier);
            }
            
            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();
            
            return $"Successfully imported {suppliers.Count}";
        }
        
        // 02. Import Parts
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            ICollection<Part> parts = new List<Part>();

            ImportPartDto[]? partDtos = XmlSerializerWrapper
                .Deserialize<ImportPartDto[]>(inputXml, "Parts");
            
            if (partDtos == null)
                return string.Empty;
            
            int[] existingSupplierIds = context
                .Suppliers
                .AsNoTracking()
                .Select(s => s.Id)
                .ToArray();

            foreach (ImportPartDto partDto in partDtos)
            {
                if (!IsValid(partDto))
                    continue;
                
                bool isValidPriceValue = decimal
                    .TryParse(partDto.Price, NumberStyles.Any,
                        CultureInfo.InvariantCulture, out decimal price);
                
                bool isValidQuantityValue = int
                    .TryParse(partDto.Quantity, out int quantity);
                
                bool isValidSupplierId = int
                    .TryParse(partDto.SupplierId, out int supplierId);
                
                if (!isValidPriceValue || !isValidQuantityValue || !isValidSupplierId || 
                    price < 0 || quantity < 0)
                    continue;
                
                if (!existingSupplierIds.Contains(supplierId))
                    continue;

                Part newPart = new Part()
                {
                    Name = partDto.Name,
                    Price = price,
                    Quantity = quantity,
                    SupplierId = supplierId
                };
                
                parts.Add(newPart);
            }
            
            context.Parts.AddRange(parts);
            context.SaveChanges();
            
            return $"Successfully imported {parts.Count}";
        }
        
      // 03. Import Cars
      public static string ImportCars(CarDealerContext context, string inputXml)
      {
          ICollection<Car> cars = new List<Car>();
          ICollection<PartCar> partsCars = new List<PartCar>();

          ImportCarDto[]? carDtos = XmlSerializerWrapper
              .Deserialize<ImportCarDto[]>(inputXml, "Cars");

          if (carDtos == null)
              return string.Empty;

          int[] existingPartIds = context
              .Parts
              .AsNoTracking()
              .Select(p => p.Id)
              .ToArray();

          foreach (ImportCarDto carDto in carDtos)
          {
              if (!IsValid(carDto))
                  continue;

              bool isValidDistance = long
                  .TryParse(carDto.TraveledDistance, out long traveledDistance);

              if (!isValidDistance || traveledDistance < 0)
                  continue;

              Car newCar = new Car()
              {
                  Make = carDto.Make,
                  Model = carDto.Model,
                  TraveledDistance = traveledDistance
              };

              cars.Add(newCar);
              
              string[] parts = carDto.Parts
                  .Select(p => p.Id)
                  .Distinct()              
                  .ToArray();
              
              foreach (string part in parts)
              {
                  bool isValidPartId = int
                      .TryParse(part, out int partId);

                  if (!isValidPartId)
                      continue;

                  if (!existingPartIds.Contains(partId))
                      continue;

                  PartCar newPartCar = new PartCar()
                  {
                      PartId = partId,
                      Car = newCar
                  };

                  partsCars.Add(newPartCar);
              }
          }

          // context.Cars.AddRange(cars); // Actually it's not needed, since EF will find the new cars from mapping entities
          context.PartsCars.AddRange(partsCars);
          context.SaveChanges();

          return $"Successfully imported {cars.Count}";
      }
      
      // 04. Import Customers
      public static string ImportCustomers(CarDealerContext context, string inputXml)
      {
          ICollection<Customer> customers = new List<Customer>();

          ImportCustomerDto[]? customerDtos = XmlSerializerWrapper
              .Deserialize<ImportCustomerDto[]>(inputXml, "Customers");

          if (customerDtos == null)
              return string.Empty;

          foreach (ImportCustomerDto customerDto in customerDtos)
          {
              if (!IsValid(customerDto))
                  continue;

              bool isValidDate = DateTime
                  .TryParseExact(customerDto.Birthdate, "yyyy-MM-dd'T'HH:mm:ss",
                      CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthdate);

              bool isYoungDriverValid = bool
                  .TryParse(customerDto.IsYoungDriver, out bool isYoungDriver);

              if (!isValidDate || !isYoungDriverValid)
                  continue;

              Customer newCustomer = new Customer()
              {
                  Name = customerDto.Name,
                  BirthDate = birthdate,
                  IsYoungDriver = isYoungDriver
              };

              customers.Add(newCustomer);
          }

          context.Customers.AddRange(customers);
          context.SaveChanges();

          return $"Successfully imported {customers.Count}";
      }
      
      // 05. Import Sales
      public static string ImportSales(CarDealerContext context, string inputXml)
      {
          ICollection<Sale> sales = new List<Sale>();

          ImportSaleDto[]? saleDtos = XmlSerializerWrapper
              .Deserialize<ImportSaleDto[]>(inputXml, "Sales");

          if (saleDtos == null)
              return string.Empty;

          int[] existingCarIds = context
              .Cars
              .AsNoTracking()
              .Select(c => c.Id)
              .ToArray();

          foreach (ImportSaleDto saleDto in saleDtos)
          {
              if (!IsValid(saleDto))
                  continue;
              
              bool isCarIdValid = int
                  .TryParse(saleDto.CarId, out int carId);

              bool isCustomerIdValid = int
                  .TryParse(saleDto.CustomerId, out int customerId);
              
              bool isDiscountValid = decimal
                  .TryParse(saleDto.Discount, NumberStyles.Any,
                      CultureInfo.InvariantCulture, out decimal discount);

              if (!isCarIdValid || !isCustomerIdValid || !isDiscountValid ||
                  !existingCarIds.Contains(carId))
                  continue;

              Sale newSale = new Sale()
              {
                  Discount = discount,
                  CarId = carId,
                  CustomerId = customerId,
              };

              sales.Add(newSale);
          }

          context.Sales.AddRange(sales);
          context.SaveChanges();

          return $"Successfully imported {sales.Count}";
      }
      
      // 06. Export Cars With Distance
      public static string GetCarsWithDistance(CarDealerContext context)
      {
          ExportCarDto[] cars = context
              .Cars
              .AsNoTracking()
              .Where(c => c.TraveledDistance > 2_000_000)
              .OrderBy(c => c.Make)
              .ThenBy(c => c.Model)
              .Select(c => new ExportCarDto()
              {
                  Make = c.Make,
                  Model = c.Model,
                  TraveledDistance = c.TraveledDistance
              })
              .Take(10)
              .ToArray();
          
          string result = XmlSerializerWrapper
              .Serialize(cars, "cars");

          return result;
      }
      
     // 07. Export Cars from Make BMW
     public static string GetCarsFromMakeBmw(CarDealerContext context)
     {
         ExportBmwCarDto[] bmwCars = context
             .Cars
             .AsNoTracking()
             .Where(c => c.Make == "BMW")
             .OrderBy(c => c.Model)
             .ThenByDescending(c => c.TraveledDistance)
             .Select(c => new ExportBmwCarDto()
             {
                 Id = c.Id,
                 Model = c.Model,
                 TraveledDistance = c.TraveledDistance
             })
             .ToArray();

         string result = XmlSerializerWrapper
             .Serialize(bmwCars, "cars");

         return result;
     }
     
     // 08. Export Local Suppliers
     public static string GetLocalSuppliers(CarDealerContext context)
     {
         ExportSupplierDto[] suppliers = context
             .Suppliers
             .AsNoTracking()
             .Where(s => s.IsImporter == false)
             .Select(s => new ExportSupplierDto()
             {
                 Id = s.Id,
                 Name = s.Name,
                 PartsCount = s.Parts.Count
             })
             .ToArray();

         string result = XmlSerializerWrapper
             .Serialize(suppliers, "suppliers");

         return result;
     }
     
     // 09. Export Cars With Their List Of Parts
     public static string GetCarsWithTheirListOfParts(CarDealerContext context)
     {
         ExportCarDtoWithListOfParts[] carsWithParts = context
             .Cars
             .Include(c => c.PartsCars)
             .ThenInclude(pc => pc.Part)
             .AsNoTracking()
             .OrderByDescending(c => c.TraveledDistance)
             .ThenBy(c => c.Model)
             .Select(c => new ExportCarDtoWithListOfParts()
             {
                 Make = c.Make,
                 Model = c.Model,
                 TraveledDistance = c.TraveledDistance,
                 Parts = c
                     .PartsCars
                     .OrderByDescending(pc => pc.Part.Price)
                     .Select(pc => new ExportPartDto()
                     {
                         Name = pc.Part.Name,
                         Price = pc.Part.Price
                     })
                     .ToArray()
             })
             .Take(5)
             .ToArray();

         string result = XmlSerializerWrapper
             .Serialize(carsWithParts, "cars");

         return result;
     }
     
     // 10. Export Total Sales By Customer
     public static string GetTotalSalesByCustomer(CarDealerContext context)
     {
         var data = context
             .Customers
             .Include(c => c.Sales)
             .ThenInclude(s => s.Car)
             .ThenInclude(c => c.PartsCars)
             .ThenInclude(pc => pc.Part)
             .Where(c => c.Sales.Count > 0)
             .AsNoTracking()
             .ToArray()
             .Select(c => new
             {
                 FullName = c.Name,
                 BoughtCars = c.Sales.Count,
                 SpentMoney = c.Sales
                     .Select(s => s.Car.PartsCars
                         .Select(pc => pc.Part)
                         .Sum(p => p.Price))
                     .Sum(),
                 DiscountMoney = (c.IsYoungDriver ? 0.95M : 1M)
             })
             .ToArray()
             .OrderByDescending(c => c.SpentMoney)
             .ToArray();
     
         ExportCustomerDto[] customers = data
             .Select(c => new ExportCustomerDto()
             {
                 FullName = c.FullName,
                 BoughtCars = c.BoughtCars,
                 SpentMoney = (Math.Truncate(c.SpentMoney * c.DiscountMoney * 100) / 100)
                     .ToString("F2", CultureInfo.InvariantCulture)
             })
             .ToArray();
        
        string result = XmlSerializerWrapper
            .Serialize(customers, "customers");
     
        return result;
     }
     
     // 11. Export Sales With Applied Discount
     public static string GetSalesWithAppliedDiscount(CarDealerContext context)
     {
         var data = context
             .Sales
             .Include(s => s.Customer)
             .Include(s => s.Car)
             .ThenInclude(c => c.PartsCars)
             .ThenInclude(pc => pc.Part)
             .AsNoTracking()
             .Select(s => new
             {
                 CarMake = s.Car.Make,
                 CarModel = s.Car.Model,
                 CarDistance = s.Car.TraveledDistance,
                 CustomerName = s.Customer.Name,
                 Discount = s.Discount,
                 Price = s.Car.PartsCars.Sum(pc => pc.Part.Price)
             })
             .Take(10)
             .ToArray();

         ExportSaleDto[] sales = data.Select(s =>
             {
                 decimal priceWithDiscount = s.Price - (s.Price * (s.Discount / 100M));

                 return new ExportSaleDto
                 {
                     Car = new ExportCarForSaleDto
                     {
                         Make = s.CarMake,
                         Model = s.CarModel,
                         TraveledDistance = s.CarDistance
                     },
                     CustomerName = s.CustomerName,
                     Discount = s.Discount.ToString("F0", CultureInfo.InvariantCulture),
                     Price = s.Price.ToString("0.####", CultureInfo.InvariantCulture),
                     PriceWithDiscount = priceWithDiscount.ToString("0.####", CultureInfo.InvariantCulture)
                 };
             })
             .ToArray();

         return XmlSerializerWrapper.Serialize(sales, "sales");
     }
        private static void ResetAndSeedDatabase(CarDealerContext dbContext)
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            Console.WriteLine("Successfully created the Car Dealer DB");
            
            // Problem 01
            string xmlFile = "suppliers.xml";
            string xmlFileText = ReadXmlDatasetFileContents(xmlFile);
            string result = ImportSuppliers(dbContext, xmlFileText);
            
            Console.WriteLine(result);
            
            // Problem 02
            xmlFile = "parts.xml";
            xmlFileText = ReadXmlDatasetFileContents(xmlFile);
            result = ImportParts(dbContext, xmlFileText);
            
            Console.WriteLine(result);
            
            // Problem 03
            xmlFile = "cars.xml";
            xmlFileText = ReadXmlDatasetFileContents(xmlFile);
            result = ImportCars(dbContext, xmlFileText);
            
            Console.WriteLine(result);
            
            // Problem 04
            xmlFile = "customers.xml";
            xmlFileText = ReadXmlDatasetFileContents(xmlFile);
            result = ImportCustomers(dbContext, xmlFileText);
            
            Console.WriteLine(result);
            
            // Problem 05
            xmlFile = "sales.xml";
            xmlFileText = ReadXmlDatasetFileContents(xmlFile);
            result = ImportSales(dbContext, xmlFileText);
            
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
        
        private static void PrintResult(CarDealerContext dbContext)
        {
            // Problem 06
            string fileName = "cars.xml";
            string result = GetCarsWithDistance(dbContext);
            WriteSerializationResult(fileName, result);
            
            Console.WriteLine(result);
            
            // Problem 07
            fileName = "bmw-cars.xml";
            result = GetCarsFromMakeBmw(dbContext);
            WriteSerializationResult(fileName, result);
            
            Console.WriteLine(result);
            
            // Problem 08
            fileName = "local-suppliers.xml";
            result = GetLocalSuppliers(dbContext);
            WriteSerializationResult(fileName, result);
            
            Console.WriteLine(result);
            
            // Problem 09
            fileName = "cars-and-parts.xml";
            result = GetCarsWithTheirListOfParts(dbContext);
            WriteSerializationResult(fileName, result);
            
            Console.WriteLine(result);
            
            // Problem 10
            fileName = "customers-total-sales.xml";
            result = GetTotalSalesByCustomer(dbContext);
            WriteSerializationResult(fileName, result);
            
            Console.WriteLine(result);
            
            // Problem 11
            fileName = "sales-discounts.xml";
            result = GetSalesWithAppliedDiscount(dbContext);
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