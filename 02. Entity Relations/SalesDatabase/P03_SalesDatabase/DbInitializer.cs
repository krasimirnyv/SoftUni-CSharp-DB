namespace P03_SalesDatabase;

using Data;
using Data.Models;

public static class DbInitializer
{
    private static readonly Random _random = new Random();

    public static void Seed(SalesContext context)
    {
        // if there are any data, seeding is canceled
        if (context.Customers.Any() ||
            context.Products.Any() ||
            context.Stores.Any())
        {
            return;
        }
        
        Customer[] customers = Enumerable
            .Range(1, 10)
            .Select(i => new Customer
            {
                Name = $"Customer {i}",
                Email = $"customer.{i}@example.com",
                CreditCardNumber = $"{_random.Next(100_000_000, 999_999_999)}"
            })
            .ToArray();
        
        context.Customers.AddRange(customers);

        Product[] products = Enumerable
            .Range(1, 10)
            .Select(i => new Product
            {
                Name = $"Product {i}",
                Quantity = _random.Next(1, 1000),
                Price = Math.Round((decimal)_random.NextDouble() * 100, 2)
            })
            .ToArray();
        
        context.Products.AddRange(products);

        Store[] stores = Enumerable
            .Range(1, 10)
            .Select(i => new Store
            {
                Name = $"Store №{i}"
            })
            .ToArray();
        
        context.Stores.AddRange(stores);
        
        context.SaveChanges();
        
        ICollection<Sale> sales = new List<Sale>();
        for (int i = 0; i < 20; i++)
        {
            Sale sale = new Sale
            {
                ProductId = products[_random.Next(products.Length)].ProductId,
                CustomerId = customers[_random.Next(customers.Length)].CustomerId,
                StoreId = stores[_random.Next(customers.Length)].StoreId,
                Date = DateTime.Now.AddDays(-_random.Next(0, 100))
            };
            
            sales.Add(sale);
        }
        
        context.Sales.AddRange(sales);
        
        context.SaveChanges();
    }
}