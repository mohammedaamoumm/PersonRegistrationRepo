using PersonRegistration.Domain.Models;

namespace PersonRegistration.Domain.Interfaces
{
    public interface IPersonRepository
    {
        void SavePerson(Person person);
        void SaveSpouse(Spouse spouse, string filePath);
    }
}
