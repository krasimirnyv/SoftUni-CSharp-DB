namespace P03_SalesDatabase;

using Data;
using Data.Models;

public static class PrintData
{
    public static void Print()
    {
        using SalesContext context = new SalesContext();

        var customers = context
            .Customers
            .Select(c => new
            {
                c.CustomerId,
                c.Name,
                c.Email,
                c.CreditCardNumber,
                Purchase = c.Sales.Count,
            })
            .ToArray();

        Console.WriteLine("Customers:");
        foreach (var c in customers)
        {
            Console.WriteLine($"\t[{c.CustomerId}] {c.Name} with Email: {c.Email}, with Card: {c.CreditCardNumber}, made {c.Purchase} purchases.");
        }

        var products = context
            .Products
            .Select(p => new
            {
                p.ProductId,
                p.Name,
                p.Price,
                p.Quantity,
                Sold = p.Sales.Count
            })
            .ToArray();

        Console.WriteLine("Products:");
        foreach (var p in products)
        {
            Console.WriteLine($"\t[{p.ProductId}] {p.Name} | quantity: {p.Quantity} | price: {p.Price}, is sold {p.Sold} times.");
        }

        var stores = context
            .Stores
            .Select(s => new
            {
                s.StoreId,
                s.Name,
                SoldUnits = s.Sales.Count
            })
            .ToArray();

        Console.WriteLine("Stores:");
        foreach (var s in stores)
        {
            Console.WriteLine($"\t[{s.StoreId}] {s.Name} sold {s.SoldUnits} units.");
        }

        var sales = context
            .Sales
            .Select(s => new
            {
                s.SaleId,
                s.Date,
                ProductName = s.Product.Name,
                CustomerName = s.Customer.Name,
                StoreName = s.Store.Name
            })
            .ToArray();

        Console.WriteLine("Sales:");
        foreach (var s in sales)
        {
            Console.WriteLine($"On this date: {s.Date}, sale with ID: {s.SaleId}, customer {s.CustomerName} bought product {s.ProductName} in store {s.StoreName}");
        }
    }
}