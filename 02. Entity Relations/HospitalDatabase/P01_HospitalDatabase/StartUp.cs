using Microsoft.EntityFrameworkCore;
using P01_HospitalDatabase.Data;

namespace P01_HospitalDatabase;

public class StartUp
{
    static void Main(string[] args)
    {
        try
        {
            using HospitalContext context = new HospitalContext();
            context.Database.Migrate();
            
            new HospitalConsole().Run();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
}