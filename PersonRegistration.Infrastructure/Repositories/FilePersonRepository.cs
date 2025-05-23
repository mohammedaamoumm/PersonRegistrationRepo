using Microsoft.Extensions.Configuration;
using PersonRegistration.Domain.Interfaces;
using PersonRegistration.Domain.Models;
using System;

public class FilePersonRepository : IPersonRepository
{
    private readonly string _mainFilePath;

    public FilePersonRepository(IConfiguration configuration)
    {
        // Read base directory and file name from config (or use defaults)
        var dataDir = configuration["DataDirectory"] ?? "C:\\people";
        var fileName = configuration["MainFileName"] ?? "People.txt";
        // Combine with current directory for cross-platform path
        _mainFilePath = Path.Combine(dataDir, fileName);
    }

    public void SavePerson(Person person)
    {
        // Ensure directory exists and append person data
        var dir = Path.GetDirectoryName(_mainFilePath)!;
        Directory.CreateDirectory(dir);
        var line = $"{person.FirstName}|{person.Surname}|{person.DateOfBirth:yyyy-MM-dd}|{person.MaritalStatus}|"
                 + $"{(person.ParentalAuthorization.HasValue ? person.ParentalAuthorization.ToString().ToLower() : "null")}|"
                 + $"{person.SpouseFilePath}";
        File.AppendAllText(_mainFilePath, line + Environment.NewLine);
    }

    public void SaveSpouse(Spouse spouse, string filePath)
    {
        // Create directory for the spouse file and save spouse data
        var dir = Path.GetDirectoryName(filePath)!;
        Directory.CreateDirectory(dir);
        var line = $"{spouse.FirstName}|{spouse.Surname}|{spouse.DateOfBirth:yyyy-MM-dd} | {spouse.MaritalStatus}";
        File.WriteAllText(filePath, line + Environment.NewLine);
    }
}