using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _config;

        public PersonRegistrationService(
            IPersonRepository repository,
            IPersonValidator validator,
            ILogger<PersonRegistrationService> logger,
            IConfiguration config)
        {
            _repository = repository;
            _validator = validator;
            _logger = logger;
            _config = config;
        }

        public string GetSpouseDirectory()
        {
            // Combine the configured base path  with the "spouses" folder
            var dataDirectory = _config["DataDirectory"];
            if (string.IsNullOrWhiteSpace(dataDirectory))
                throw new InvalidOperationException("Missing 'DataDirectory' in configuration.");

            var subFolder = _config["SpouseDirectory"] ?? "spouses";

            return Path.Combine(dataDirectory, subFolder);
        }

        public bool RegisterPerson(Person person, Spouse spouse, string spouseFilePath)
        {
            _logger.LogInformation("Attempting to register person: {FirstName} {LastName}", person.FirstName, person.Surname);
            if (!_validator.IsEligibleForRegistration(person))
            {
                _logger.LogWarning("Person {FirstName} {LastName} is not eligible for registration.", person.FirstName, person.Surname);
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
