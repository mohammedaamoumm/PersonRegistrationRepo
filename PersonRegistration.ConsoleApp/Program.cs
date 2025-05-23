using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using PersonRegistration.Application.Services;
using PersonRegistration.Application.Validators;
using PersonRegistration.Domain.Enums;
using PersonRegistration.Domain.Interfaces;
using PersonRegistration.Domain.Models;
using PersonRegistration.Domain.Validators;
using PersonRegistration.Infrastructure.Repositories;

var services = ConfigureServices();
var serviceProvider = services.BuildServiceProvider();

try
{
    var registrationService = serviceProvider.GetRequiredService<PersonRegistrationService>();
    var person = PromptPersonInput(out Spouse? spouse, out string? spouseFilePath);

    if (person != null)
    {
        bool result = registrationService.RegisterPerson(person, spouse, spouseFilePath);
        Console.WriteLine(result ? "Registration successful." : "Registration failed.");
    }
}
catch (Exception ex)
{
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An unexpected error occurred.");
    Console.WriteLine("An unexpected error occurred. Please check the logs.");
}

static IServiceCollection ConfigureServices()
{
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true)
        .Build();

    var services = new ServiceCollection();

    services.AddLogging(config =>
    {
        config.AddConsole();
        config.SetMinimumLevel(LogLevel.Information);
    });

    services.AddSingleton<IConfiguration>(configuration);
    services.AddSingleton<IPersonRepository, FilePersonRepository>();
    services.AddSingleton<IPersonValidator, PersonValidator>();
    services.AddSingleton<PersonRegistrationService>();

    return services;
}

static Person? PromptPersonInput(out Spouse? spouse, out string? spouseFilePath)
{
    spouse = null;
    spouseFilePath = null;

    try
    {
        Console.WriteLine("Enter First Name:");
        string firstName = Console.ReadLine();

        Console.WriteLine("Enter Surname:");
        string surname = Console.ReadLine();

        Console.WriteLine("Enter Date of Birth (yyyy-MM-dd):");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime dob))
        {
            Console.WriteLine("Invalid date format.");
            return null;
        }

        int age = (int)((DateTime.Now - dob).TotalDays / 365.25);

        if (age < 16)
        {
            Console.WriteLine("Registration denied. Must be at least 16 years old.");
            return null;
        }

        bool? parentalAuthorization = null;
        if (age < 18)
        {
            Console.WriteLine("My parents allow registration (yes/no):");
            string authInput = Console.ReadLine()?.Trim().ToLower();

            if (authInput == "yes")
            {
                parentalAuthorization = true;
            }
            else if (authInput == "no")
            {
                parentalAuthorization = false;
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter 'yes' or 'no'.");
                return null;
            }

            if (!parentalAuthorization.Value)
            {
                Console.WriteLine("Registration denied without parental authorization.");
                return null;
            }
        }

        Console.WriteLine("Enter Marital Status (Single/Married):");
        string statusInput = Console.ReadLine();
        if (!Enum.TryParse<MaritalStatus>(statusInput, true, out var status))
        {
            Console.WriteLine("Invalid marital status.");
            return null;
        }

        if (status == MaritalStatus.Married)
        {
            Console.WriteLine("Enter Spouse First Name:");
            string sFirstName = Console.ReadLine();

            Console.WriteLine("Enter Spouse Surname:");
            string sSurname = Console.ReadLine();

            Console.WriteLine("Enter Spouse Date of Birth (yyyy-MM-dd):");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime sDob))
            {
                Console.WriteLine("Invalid spouse date of birth.");
                return null;
            }

            spouse = new Spouse
            {
                FirstName = sFirstName,
                Surname = sSurname,
                DateOfBirth = sDob,
                MaritalStatus = MaritalStatus.Married
            };

            spouseFilePath = $"c:/people/spouses/{sFirstName}_{sSurname}_{Guid.NewGuid():N}.txt";
        }

        return new Person
        {
            FirstName = firstName,
            Surname = surname,
            DateOfBirth = dob,
            MaritalStatus = status,
            ParentalAuthorization = parentalAuthorization
        };
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error occurred during input: {ex.Message}");
        spouse = null;
        spouseFilePath = null;
        return null;
    }
}