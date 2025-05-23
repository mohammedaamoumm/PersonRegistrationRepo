using Microsoft.Extensions.Configuration;
using PersonRegistration.Domain.Interfaces;
using PersonRegistration.Domain.Models;
using System.Text;

namespace PersonRegistration.Infrastructure.Repositories
{
    public class FilePersonRepository : IPersonRepository
    {
        private readonly string _mainFilePath;

        public FilePersonRepository(IConfiguration configuration)
        {
            _mainFilePath = configuration["MainFilePath"] ?? "c:/people/People.txt";
        }

        public void SavePerson(Person person)
        {
            var line = $"{person.FirstName}|{person.Surname}|{person.DateOfBirth:dd-MM-yyyy}|{person.MaritalStatus}|{(person.ParentalAuthorization.HasValue ? person.ParentalAuthorization.ToString().ToLower() : "null")}|{person.SpouseFilePath}";
            Directory.CreateDirectory(Path.GetDirectoryName(_mainFilePath)!);
            File.AppendAllText(_mainFilePath, line + Environment.NewLine);
        }

        public void SaveSpouse(Spouse spouse, string filePath)
        {
            var line = $"{spouse.FirstName}|{spouse.Surname}|{spouse.DateOfBirth:dd-MM-yyyy}|{spouse.MaritalStatus}";
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            File.WriteAllText(filePath, line);
        }
    }
}
