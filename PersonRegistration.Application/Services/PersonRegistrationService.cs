// Updated: Application/Services/PersonRegistrationService.cs
using Microsoft.Extensions.Logging;
using PersonRegistration.Domain.Interfaces;
using PersonRegistration.Domain.Models;
using PersonRegistration.Domain.Validators;

namespace PersonRegistration.Application.Services
{
    public class PersonRegistrationService
    {
        private readonly IPersonRepository _repository;
        private readonly IPersonValidator _validator;
        private readonly ILogger<PersonRegistrationService> _logger;

        public PersonRegistrationService(IPersonRepository repository, IPersonValidator validator, ILogger<PersonRegistrationService> logger)
        {
            _repository = repository;
            _validator = validator;
            _logger = logger;
        }

        public bool RegisterPerson(Person person, Spouse? spouse, string? spouseFilePath)
        {
            _logger.LogInformation("Attempting to register person: {FirstName} {LastName}", person.FirstName, person.Surname);

            if (!_validator.IsEligibleForRegistration(person))
            {
                _logger.LogWarning("Registration denied for {FirstName} {LastName} due to validation failure.", person.FirstName, person.Surname);
                return false;
            }

            if (spouse != null && spouseFilePath != null)
            {
                _repository.SaveSpouse(spouse, spouseFilePath);
                person.SpouseFilePath = spouseFilePath;
                _logger.LogInformation("Spouse details saved to {SpouseFilePath}", spouseFilePath);
            }

            _repository.SavePerson(person);
            _logger.LogInformation("Person {FirstName} {LastName} successfully registered.", person.FirstName, person.Surname);

            return true;
        }
    }
}
