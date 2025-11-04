using Microsoft.EntityFrameworkCore;

namespace AcademicRecordsApp;

using Data;

class StartUp
{
    static void Main(string[] args)
    {
        /* Often used during development before initial application release */
        if (Environment.GetEnvironmentVariables().Contains("DEV"))
        {
            /* Never use with PROD database */
            AcademicRecordsDbContext dbContext = new AcademicRecordsDbContext();
            dbContext.Database.Migrate();
        }
    }
}