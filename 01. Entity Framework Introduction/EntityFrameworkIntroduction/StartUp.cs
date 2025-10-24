using System.Globalization;
using System.Text;

namespace SoftUni;

using Data;
using Models;

public class StartUp
{
    static void Main()
    {
        try
        {
            using SoftUniContext context = new SoftUniContext();

            string result = RemoveTown(context);
            
            Console.WriteLine(result);
        }
        catch (Exception e)
        {
            Console.WriteLine($"## Unexpected Error occured: {e.Message}");
        }
    }

    // 03. Employees Full Information
    public static string GetEmployeesFullInformation(SoftUniContext context)
    {
        var employees = context
            .Employees
            .OrderBy(e => e.EmployeeId)
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.MiddleName,
                e.JobTitle,
                e.Salary
            })
            .ToArray();

        StringBuilder sb = new StringBuilder();
        foreach (var e in employees)
        {
            sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:F2}");
        }
        
        return sb.ToString().TrimEnd();
    }

    // 04. Employees with Salary Over 50 000
    public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
    {
        var employees = context
            .Employees
            .OrderBy(e => e.FirstName)
            .Where(e => e.Salary > 50000)
            .Select(e => new
            {
                e.FirstName,
                e.Salary
            });

        StringBuilder sb = new StringBuilder();
        foreach (var e in employees)
        {
            sb.AppendLine($"{e.FirstName} - {e.Salary:F2}");
        }
        
        return sb.ToString().TrimEnd();
    }
    
    // 05. Employees from Research and Development
    public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
    {
        var employees = context
            .Employees
            .Where(e => e.Department.Name == "Research and Development")
            .OrderBy(e => e.Salary)
            .ThenByDescending(e => e.FirstName)
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                DepartmentName = e.Department.Name,
                e.Salary
            });

        StringBuilder sb = new StringBuilder();
        foreach (var e in employees)
        {
            sb.AppendLine($"{e.FirstName} {e.LastName} from {e.DepartmentName} - ${e.Salary:F2}");
        }
        
        return sb.ToString().TrimEnd();
    }
    
    // 06. Adding a New Address and Updating Employee
    public static string AddNewAddressToEmployee(SoftUniContext context)
    {
        Employee nakovEmployee = context
            .Employees
            .First(e => e.LastName == "Nakov");

        Address newAddress = new Address()
        {
            AddressText = "Vitoshka 15",
            TownId = 4
        };

        nakovEmployee.Address = newAddress;

        context.SaveChanges();

        string[] employeesAddress = context
            .Employees
            .OrderByDescending(e => e.AddressId)
            .Select(e => e.Address.AddressText)
            .Take(10)
            .ToArray();

        return string.Join(Environment.NewLine, employeesAddress);
    }
    
    // 07. Employees and Projects
    public static string GetEmployeesInPeriod(SoftUniContext context)
    {
        var employees = context
            .Employees
            .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    ManagerFirstName = e.Manager!.FirstName,
                    ManagerLastName = e.Manager.LastName,
                    Projects = e.EmployeesProjects
                        .Where(p =>
                            p.Project.StartDate.Year >= 2001 &&
                            p.Project.StartDate.Year <= 2003)
                        .Select(p => new
                        {
                            ProjectName = p.Project.Name,
                            StartDate = p.Project.StartDate,
                            EndDate = p.Project.EndDate
                        })
                        .ToArray()
                })
            .Take(10)
            .ToArray();

        StringBuilder sb = new StringBuilder();

        foreach (var e in employees)
        {
            sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");

            foreach (var p in e.Projects)
            {
                string startDate = p.StartDate
                    .ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

                string endDate = p.EndDate
                    .HasValue
                    ? p.EndDate
                        .Value
                        .ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                    : "not finished";

                sb.AppendLine($"--{p.ProjectName} - {startDate} - {endDate}");
            }
        }
        
        return sb.ToString().Trim();
    }
    
    // 08. Addresses by Town
    public static string GetAddressesByTown(SoftUniContext context)
    {
        var addresses = context
            .Addresses
            .OrderByDescending(a => a.Employees.Count)
            .ThenBy(a => a.Town.Name)
            .ThenBy(a => a.AddressText)
            .Select(a => new
            {
                a.AddressText,
                TownName = a.Town.Name,
                EmployeeCount =  a.Employees.Count
            })
            .Take(10);

        StringBuilder sb = new StringBuilder();
        foreach (var a in addresses)
        {
            sb.AppendLine($"{a.AddressText}, {a.TownName} - {a.EmployeeCount} employees");
        }
        
        return sb.ToString().TrimEnd();
    }
    
    // 09. Employee 147
    public static string GetEmployee147(SoftUniContext context)
    {
        var employee = context
            .Employees
            .Where(e => e.EmployeeId == 147)
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.JobTitle,
                Projects = e.EmployeesProjects
                    .OrderBy(p => p.Project.Name) 
                    .Select(p => p.Project.Name)
                    .ToArray()
            })
            .First();

        StringBuilder sb = new StringBuilder();

        sb.Append($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");
        foreach (var projectName in employee.Projects)
        {
            sb.AppendLine();
            sb.Append($"{projectName}");
        }

        return sb.ToString();
    }
    
    // 10. Departments with More Than 5 Employees
    public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
    {
        var departments = context
            .Departments
            .Where(d => d.Employees.Count > 5)
            .OrderBy(d => d.Employees.Count)
            .ThenBy(d => d.Name)
            .Select(d => new
            {
                d.Name,
                ManagerFirstName = d.Manager.FirstName,
                ManagerLastName = d.Manager.LastName,
                Employees = context
                    .Employees
                    .Where(e => e.Department.Name == d.Name)
                    .Select(e => new
                    {
                        e.FirstName,
                        e.LastName,
                        e.JobTitle
                    })
                    .OrderBy(e => e.FirstName)
                    .ThenBy(e => e.LastName)
                    .ToArray()
            })
            .ToArray();

        StringBuilder sb = new StringBuilder();
        foreach (var d in departments)
        {
            sb.AppendLine($"{d.Name} - {d.ManagerFirstName} {d.ManagerLastName}");
            foreach (var e in d.Employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
            }
        }
        
        return sb.ToString().TrimEnd();
    }
    
    // 11. Find Latest 10 Projects
    public static string GetLatestProjects(SoftUniContext context)
    {
        var projects = context
            .Projects
            .OrderByDescending(p => p.StartDate)
            .Select(p => new
            {
                p.Name,
                p.Description,
                p.StartDate
            })
            .Take(10)
            .OrderBy(p => p.Name);

        StringBuilder sb = new StringBuilder();
        foreach (var p in projects)
        {
            sb.AppendLine($"{p.Name}");
            sb.AppendLine($"{p.Description}");

            string startDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
            sb.AppendLine($"{startDate}");
        }
        
        return sb.ToString().TrimEnd();
    }
    
    // 12. Increase Salaries
    public static string IncreaseSalaries(SoftUniContext context)
    {
        var employeesToUpdate = context
            .Employees
            .Where(e =>
                e.Department.Name == "Engineering" ||
                e.Department.Name == "Tool Design" ||
                e.Department.Name == "Marketing" ||
                e.Department.Name == "Information Services")
            .ToArray();

        foreach (Employee employee in employeesToUpdate)
        {
            employee.Salary *= 1.12m;
        }
        
        context.SaveChanges();
        
        var employees = employeesToUpdate
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.Salary
            })
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .ToArray();

        StringBuilder sb = new StringBuilder();
        foreach (var e in employees)
        {
            sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:F2})");
        }
        
        return sb.ToString().TrimEnd();
    }
    
    // 13. Find Employees by First Name Starting With "Sa"
    public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
    {
        var employees = context
            .Employees
            .Where(e => e.FirstName.ToLower().StartsWith("sa"))
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.JobTitle,
                e.Salary
            })
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .ToArray();

        StringBuilder sb = new StringBuilder();
        foreach (var e in employees)
        {
            sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:F2})");
        }
        
        return sb.ToString().TrimEnd();
    }
    
    // 14. Delete Project by Id
    public static string DeleteProjectById(SoftUniContext context)
    {
        Project projectToDelete = context
            .Projects
            .First(p => p.ProjectId == 2);
        
        context
            .EmployeesProjects
            .RemoveRange(context
                .EmployeesProjects
                .Where(ep => ep.ProjectId == 2));
        
        context
            .Projects
            .Remove(projectToDelete);

        context.SaveChanges();

        var projects = context
            .Projects
            .Select(p => p.Name)
            .Take(10)
            .ToArray();

        StringBuilder sb = new StringBuilder();
        foreach (string project in projects)
        {
            sb.AppendLine(project);
        }
        
        return sb.ToString().TrimEnd();
    }
    
    // 15. Remove Town
    public static string RemoveTown(SoftUniContext context)
    {
        Town townToDelete = context
            .Towns
            .First(t => t.Name == "Seattle");

        Address[] addressesInThisTown = context
            .Addresses
            .Where(a => a.Town.Name == "Seattle")
            .ToArray();
        
        int addressesCount = addressesInThisTown.Length;
        
        Employee[] employeesInThisTown = context
            .Employees
            .Where(e => e.Address.Town.Name == "Seattle")
            .ToArray();

        foreach (Employee employee in employeesInThisTown)
        {
            employee.AddressId = null;
        }
        
        context
            .Addresses
            .RemoveRange(addressesInThisTown);
        
        context
            .Towns
            .Remove(townToDelete);

        context.SaveChanges();

        return $"{addressesCount} addresses in Seattle were deleted";
    }
}