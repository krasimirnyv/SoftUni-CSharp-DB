using System.Globalization;

using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CarDealer
{
    using Data;
    using DTOs.Import;
    using Models;
    
    public class StartUp
    {
        public static void Main()
        {
            using CarDealerContext dbContext = new CarDealerContext();

            // dbContext.Database.EnsureDeleted();
            // dbContext.Database.EnsureCreated();
            // Console.WriteLine("Successfully created the Car Dealer DB");
            
            string jsonFileDirPath = Path
                .Combine(Directory.GetCurrentDirectory(), "../../../Datasets/");

            // Problem 01
            string jsonFileName = "suppliers.json";
            string jsonFileText = File.ReadAllText(jsonFileDirPath + jsonFileName);

            string result = ImportSuppliers(dbContext, jsonFileText);
            Console.WriteLine(result);

            // Problem 02
            jsonFileName = "parts.json";
            jsonFileText = File.ReadAllText(jsonFileDirPath + jsonFileName);

            result = ImportParts(dbContext, jsonFileText);
            Console.WriteLine(result);

            // Problem 03
            jsonFileName = "cars.json";
            jsonFileText = File.ReadAllText(jsonFileDirPath + jsonFileName);

            result = ImportCars(dbContext, jsonFileText);
            Console.WriteLine(result);

            // Problem 04
            jsonFileName = "customers.json";
            jsonFileText = File.ReadAllText(jsonFileDirPath + jsonFileName);

            result = ImportCustomers(dbContext, jsonFileText);
            Console.WriteLine(result);

            // Problem 05
            jsonFileName = "sales.json";
            jsonFileText = File.ReadAllText(jsonFileDirPath + jsonFileName);

            result = ImportSales(dbContext, jsonFileText);
            Console.WriteLine(result);

            // Problem 06
            result = GetOrderedCustomers(dbContext);
            Console.WriteLine(result);
            
            // Problem 07
            result = GetCarsFromMakeToyota(dbContext);
            Console.WriteLine(result);
            
            // Problem 08
            result = GetLocalSuppliers(dbContext);
            Console.WriteLine(result);
            
            // Problem 09
            result = GetCarsWithTheirListOfParts(dbContext);
            Console.WriteLine(result);
            
            // Problem 10
            result = GetTotalSalesByCustomer(dbContext);
            Console.WriteLine(result);
            
            // Problem 11
            result = GetSalesWithAppliedDiscount(dbContext);
            Console.WriteLine(result);
        }

        // 01. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            ICollection<Supplier> suppliers = new List<Supplier>();

            IEnumerable<ImportSupplierDto>? supplierDtos = JsonConvert
                .DeserializeObject<ImportSupplierDto[]>(inputJson);

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
            
            return $"Successfully imported {suppliers.Count}.";
        }

        // 02. Import Parts
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            ICollection<Part> parts = new List<Part>();

            IEnumerable<ImportPartDto>? partDtos = JsonConvert
                .DeserializeObject<ImportPartDto[]>(inputJson);
            
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
            
            return $"Successfully imported {parts.Count}.";
        }

        // 03. Import Cars
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            ICollection<Car> cars = new List<Car>();
            ICollection<PartCar> partsCars = new List<PartCar>();
            
            IEnumerable<ImportCarDto>? carDtos = JsonConvert
                .DeserializeObject<ImportCarDto[]>(inputJson);
            
            if (carDtos == null)
                return string.Empty;

            HashSet<int> existingPartIds = context
                .Parts
                .AsNoTracking()
                .Select(p => p.Id)
                .ToHashSet();
            
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

                foreach (string part in carDto.PartsIds.Distinct())
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
            
            return $"Successfully imported {cars.Count}.";
        }
        
        // 04. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            ICollection<Customer> customers = new List<Customer>();
            
            IEnumerable<ImportCustomerDto>? customerDtos = JsonConvert
                .DeserializeObject<ImportCustomerDto[]>(inputJson);

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
            
            return $"Successfully imported {customers.Count}.";
        }
        
        // 05. Import Sales
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            ICollection<Sale> sales = new List<Sale>();
            
            IEnumerable<ImportSaleDto>? saleDtos = JsonConvert
                .DeserializeObject<ImportSaleDto[]>(inputJson);
            
            if (saleDtos == null)
                return string.Empty;
        
            HashSet<int> existingCarIds = context
                .Cars
                .AsNoTracking()
                .Select(c => c.Id)
                .ToHashSet();
        
            HashSet<int> existingCustomerIds = context
                .Customers
                .AsNoTracking()
                .Select(c => c.Id)
                .ToHashSet();
            
            foreach (ImportSaleDto saleDto in saleDtos)
            {
                if (!IsValid(saleDto))
                    continue;
                
                bool isDiscountValid = decimal
                    .TryParse(saleDto.Discount, NumberStyles.Any, 
                        CultureInfo.InvariantCulture, out decimal discount);
                
                bool isCarIdValid = int
                    .TryParse(saleDto.CarId, out int carId);
                
                bool isCustomerIdValid = int
                    .TryParse(saleDto.CustomerId, out int customerId);
                
                if (!isDiscountValid || !isCarIdValid || !isCustomerIdValid || 
                    !existingCarIds.Contains(carId) || 
                    !existingCustomerIds.Contains(customerId))
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
            
            return $"Successfully imported {sales.Count}.";
        }
        
        // 06. Export Ordered Customers
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context
                .Customers
                .AsNoTracking()
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new
                {
                    c.Name,
                    c.BirthDate,
                    c.IsYoungDriver
                })
                .ToArray();

            var customersToExport = customers
                .Select(c => new
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToArray();
            
            string json = JsonConvert
                .SerializeObject(customersToExport, Formatting.Indented);

            return json;
        }
        
        // 07. Export Cars From Make Toyota
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context
                .Cars
                .AsNoTracking()
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .Select(c => new
                {
                    c.Id,
                    c.Make,
                    c.Model,
                    c.TraveledDistance
                });
            
            string json = JsonConvert
                .SerializeObject(cars, Formatting.Indented);

            return json;
        }
        
        // 08. Export Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context
                .Suppliers
                .AsNoTracking()
                .Where(s => s.IsImporter == false)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    PartsCount = s.Parts.Count
                });
            
            string json = JsonConvert
                .SerializeObject(suppliers, Formatting.Indented);
            
            return json;
        }
        
        // 09. Export Cars With Their List Of Parts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsWithParts = context
                .Cars
                .Include(c => c.PartsCars)
                .ThenInclude(pc => pc.Part)
                .AsNoTracking()
                .Select(c => new
                {
                    c.Make,
                    c.Model,
                    c.TraveledDistance,
                    Parts = c
                        .PartsCars
                        .Select(pc => new
                        {
                            pc.Part.Name,
                            pc.Part.Price
                        })
                        .ToArray()
                })
                .ToArray();

            var carsToExport = carsWithParts
                .Select(c => new
                {
                    car = new
                    {
                        c.Make,
                        c.Model,
                        c.TraveledDistance,
                    },
                    parts = c.Parts
                        .Select(cp => new
                        {
                            cp.Name,
                            Price = cp.Price.ToString("F2")
                        })
                        .ToArray()
                })
                .ToArray();
            
            string json = JsonConvert
                .SerializeObject(carsToExport, Formatting.Indented);
            
            return json;
        }
        
        // 10. Export Total Sales By Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context
                .Customers
                .Include(c => c.Sales)
                .ThenInclude(s => s.Car)
                .ThenInclude(c => c.PartsCars)
                .ThenInclude(pc => pc.Part)
                .AsNoTracking()
                .Where(c => c.Sales.Count > 0)
                .Select(c => new
                {
                    c.Name,
                    CarAmount = c.Sales.Count,
                    Money = c.Sales
                        .Select(s => s.Car.PartsCars
                            .Sum(p => p.Part.Price)),
                })
                .ToArray();

            var customersToExport = customers
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.CarAmount,
                    spentMoney = c.Money.Sum()
                })
                .OrderByDescending(c => c.spentMoney)
                .ThenByDescending(c => c.boughtCars)
                .ToArray();
            
            string json = JsonConvert
                .SerializeObject(customersToExport, Formatting.Indented);
            
            return json;
        }
        
        // 11. Export Sales With Applied Discount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var top10Sales = context
                .Sales
                .Include(s => s.Customer)
                .Include(s => s.Car)
                .ThenInclude(c => c.PartsCars)
                .ThenInclude(pc => pc.Part)
                .AsNoTracking()
                .Select(s => new
                {
                    Car = new
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TraveledDistance
                    },
                    CustomerName = s.Customer.Name,
                    Discount = s.Discount,
                    Price = s.Car.PartsCars
                        .Select(ps => ps.Part)
                        .Sum(p => p.Price)
                })
                .Take(10)
                .ToArray();

            var salesToExport = top10Sales
                .Select(s => new
                {
                    car = s.Car,
                    customerName = s.CustomerName,
                    discount = s.Discount.ToString("F2"),
                    price = s.Price.ToString("F2"),
                    priceWithDiscount = (s.Price - (s.Price * (s.Discount / 100))).ToString("F2")
                })
                .ToArray();
            
            string json = JsonConvert
                .SerializeObject(salesToExport, Formatting.Indented);
            
            return json;
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