# Person Registration Console Application

The **Person Registration** console application collects personal details from the user and stores them in text files. It prompts for the first name, surname, date of birth, and marital status. If the user is under 18, parental authorization is required, if not authorized, registration is denied outright. Married users are prompted to enter their spouse’s information, which is saved in a separate file. Valid registrations are appended to `c:\people\People.txt`, and any spouse details are saved under `c:\people\spouses\[FirstName]_[Surname]_[suffix].txt`. (Suffixes can be added to the filename to avoid name conflicts.) Logging is enabled to trace the registration steps and outcomes.

## Setup and Dependencies

- **.NET SDK**: Install [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or later, as this project targets .NET 8.0. You can verify your installation with `dotnet --version`.
- **Configuration File**: The project includes an `appsettings.json` file (in the ConsoleApp folder) where the main data file path is set (`"MainFilePath": "c:/people/People.txt"`). You can modify this path if needed.
- **NuGet Packages**: Required packages are referenced in the project files. Key dependencies include:
  - `Microsoft.Extensions.DependencyInjection` and `Microsoft.Extensions.Hosting` for dependency injection and configuration.
  - `Microsoft.Extensions.Logging` for logging support.
  - `Microsoft.Extensions.Configuration` for reading `appsettings.json`.
  - (Test project uses xUnit, Moq, and FluentAssertions, but those are only needed if running tests.)
- **Platform Note**: The default file paths use Windows-style `C:\` directories. Ensure the application has permission to create and write to `C:\people` and its subfolders. On other platforms or drives, update the paths in `appsettings.json` or the code as needed.

## Folder Structure

The solution is organized into multiple projects following a layered architecture:

```
PersonRegistration/
├── PersonRegistration.ConsoleApp/    # Entry point (Program.cs) and UI interactions
├── PersonRegistration.Application/   # Business logic, services, and validators
├── PersonRegistration.Domain/        # Domain models (Person, Spouse), enums, interfaces
├── PersonRegistration.Infrastructure/  # Data persistence (file repository implementation)
├── PersonRegistration.Tests/         # Unit tests for the application logic
└── README.md                        # Project documentation
```

## Build and Run

To compile and run the application using the .NET CLI, follow these steps:

1. **Restore and Build**: From the solution root directory, run:
    ```bash
    dotnet restore
    dotnet build
    ```
2. **Run the Console Application**:
    ```bash
    dotnet run --project PersonRegistration.ConsoleApp/PersonRegistration.ConsoleApp.csproj
    ```

## Example Interaction

```
Enter First Name:
John
Enter Surname:
Doe
Enter Date of Birth (yyyy-MM-dd):
2007-04-10
Do you have parental authorization? (yes/no):
yes
Enter Marital Status (Single/Married):
Married
Enter Spouse First Name:
Jane
Enter Spouse Surname:
Doe
Enter Spouse Date of Birth (yyyy-MM-dd):
2005-09-30
Registration successful.
```

## Maintainability and Extensibility

- **Layered Architecture**: Separation of concerns across domain, application, infrastructure, and UI layers.
- **Dependency Injection**: Services are registered via `Microsoft.Extensions.DependencyInjection`.
- **Configurability**: Paths and settings are externalized in `appsettings.json`.
- **Logging**: Console logging is integrated for traceability.
- **Unit Testing**: The application logic is testable using mocks and unit tests.
- **Extensibility**: Easy to adapt for web UI or database storage in future.
