using Microsoft.EntityFrameworkCore.Infrastructure;

namespace P01_HospitalDatabase;

using Data;
using Data.Models;

public class HospitalConsole
{
    public void Run()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("### Hospital Database Menu ###");
            
            Console.WriteLine("1. Add Patient");
            Console.WriteLine("2. View Patients as List");
            Console.WriteLine("3. Delete Patient");
            
            Console.WriteLine("0. Exit");
            
            Console.Write("Select an option: ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    AddPatient();
                    break;
                case "2":
                    ListPatients();
                    break;
                case "3":
                    Console.WriteLine("Please enter the Patient ID to delete: ");
                    int patientId = int.Parse(Console.ReadLine()!);
                    DeletePatient(patientId);
                    break;
                case "0":
                    return;
            }

            Console.WriteLine("Press 'Enter' to continue...");
            Console.ReadLine();
        }
    }

    private void AddPatient()
    {
        using HospitalContext context = new HospitalContext();

        Console.WriteLine("First name: ");
        string firstName = Console.ReadLine()!;

        Console.WriteLine("Last name: ");
        string lastName = Console.ReadLine()!;

        Console.WriteLine("Address: ");
        string address = Console.ReadLine()!;

        Console.WriteLine("Does the patient have email address? (y/n): ");
        string answerEmail = Console.ReadLine()!.ToLower();

        string? email = null;
        if (answerEmail is "y" or "yes" or "da")
        {
            Console.WriteLine("Email: ");
            email = Console.ReadLine()!;
        }

        Console.WriteLine("Does the patient have insurance? (y/n): ");
        string answerInsurance = Console.ReadLine()!.ToLower();

        bool hasInsurance = false || answerInsurance is "y" or "yes" or "da";

        Patient newPatient = new Patient()
        {
            FirstName = firstName!,
            LastName = lastName!,
            Address = address!,
            Email = email ?? string.Empty,
            HasInsurance = hasInsurance
        };

        context.Patients.Add(newPatient);
        context.SaveChanges();

        Console.WriteLine($"Patient {firstName} {lastName} with ID {newPatient.PatientId} added successfully!");
    }

    private void ListPatients()
    {
        using HospitalContext context = new HospitalContext();

        Patient[] patients = context.Patients.ToArray();

        foreach (Patient p in patients)
        {
            Console.WriteLine($"[{p.PatientId}], {p.FirstName} {p.LastName}, Has Insurance: {p.HasInsurance}");
            if (!string.IsNullOrEmpty(p.Email))
            {
                Console.WriteLine($"    Email: {p.Email}");
            }
        }
    }

    private bool DeletePatient(int patientId)
    {
        using HospitalContext context = new HospitalContext();

        Patient? patient = context
            .Patients
            .FirstOrDefault(p => p.PatientId == patientId);

        if (patient == null)
        {
            Console.WriteLine("Invalid Patient ID");
            return false;
        }

        context
            .Patients
            .Remove(patient);
        
        Console.WriteLine($"Patient with ID {patientId} deleted successfully!");
        
        context.SaveChanges();
        
        return true;
    }
}