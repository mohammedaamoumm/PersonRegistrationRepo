using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PersonRegistration.Application.Services;
using PersonRegistration.Domain.Enums;
using PersonRegistration.Domain.Interfaces;
using PersonRegistration.Domain.Models;
using System.Globalization;

namespace PersonRegistration.ConsoleApp
{
    public class PersonRegistrationHostedService : IHostedService
    {
        private readonly PersonRegistrationService _registrationService;
        private readonly IUserInteractionService _interaction;
        private readonly ILogger<PersonRegistrationHostedService> _logger;
        private readonly IConfiguration _config;


        public PersonRegistrationHostedService(
            PersonRegistrationService registrationService,
            IUserInteractionService interaction,
            ILogger<PersonRegistrationHostedService> logger, IConfiguration config)
        {
            _registrationService = registrationService;
            _interaction = interaction;
            _logger = logger;
            _config = config;

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Run the registration process (single-run console app).
            try
            {
                var person = PromptPersonInput(out Spouse? spouse, out string? spouseFilePath);
                if (person != null)
                {
                    bool success = _registrationService.RegisterPerson(person, spouse, spouseFilePath);
                    _interaction.ShowMessage(success ? "Registration successful." : "Registration failed.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during registration");
                _interaction.ShowMessage("An error occurred. Please check the logs.");
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private Person? PromptPersonInput(out Spouse? spouse, out string? spouseFilePath)
        {
            spouse = null;
            spouseFilePath = null;
            try
            {
                // First name
                var firstName = _interaction.GetInput("Enter First Name:");
                // Surname
                var surname = _interaction.GetInput("Enter Surname:");
                // Date of birth (validate format and future date)
                var dobInput = _interaction.GetInput("Enter Date of Birth (yyyy-MM-dd):");
                if (!DateTime.TryParseExact(dobInput, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                                            DateTimeStyles.None, out var dob))
                {
                    _interaction.ShowMessage("Invalid date format.");
                    return null;
                }
                if (dob > DateTime.Today)
                {
                    _interaction.ShowMessage("Date of birth cannot be in the future.");
                    return null;
                }
                // Parental authorization (if under 18)
                bool? parentalAuth = null;
                int age = (int)((DateTime.Today - dob).TotalDays / 365.25);
                if (age < 18)
                {
                    var authInput = _interaction.GetInput("My parents allow registration (yes/no):").Trim().ToLower();
                    if (authInput == "yes") parentalAuth = true;
                    else if (authInput == "no") parentalAuth = false;
                    else
                    {
                        _interaction.ShowMessage("Invalid input for parental authorization.");
                        return null;
                    }
                }
                // Marital status
                var statusInput = _interaction.GetInput("Enter Marital Status (Single/Married):");
                if (!Enum.TryParse<MaritalStatus>(statusInput, true, out var status))
                {
                    _interaction.ShowMessage("Invalid marital status.");
                    return null;
                }

                // If married, get spouse info
                if (status == MaritalStatus.Married)
                {
                    var sFirst = _interaction.GetInput("Enter Spouse First Name:");
                    var sLast = _interaction.GetInput("Enter Spouse Surname:");
                    var sDobInput = _interaction.GetInput("Enter Spouse Date of Birth (yyyy-MM-dd):");
                    if (!DateTime.TryParseExact(sDobInput, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                                                DateTimeStyles.None, out var sDob))
                    {
                        _interaction.ShowMessage("Invalid spouse date of birth.");
                        return null;
                    }
                    if (sDob > DateTime.Today)
                    {
                        _interaction.ShowMessage("Spouse date of birth cannot be in the future.");
                        return null;
                    }
                    spouse = new Spouse
                    {
                        FirstName = sFirst,
                        Surname = sLast,
                        DateOfBirth = sDob,
                        MaritalStatus = MaritalStatus.Married
                    };
                    // Build spouse file path from config directory and a GUID
                    var dataDirectory = _config["DataDirectory"];
                    var spouseDir = _registrationService.GetSpouseDirectory();

                    Directory.CreateDirectory(spouseDir);
                    spouseFilePath = Path.Combine(spouseDir, $"{sFirst}_{Guid.NewGuid():N}.txt");
                }

                // Return populated Person object
                return new Person
                {
                    FirstName = firstName,
                    Surname = surname,
                    DateOfBirth = dob,
                    MaritalStatus = status,
                    ParentalAuthorization = parentalAuth
                };
            }
            catch
            {
                spouse = null;
                spouseFilePath = null;
                return null;
            }
        }
    }
}
