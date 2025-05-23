using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PersonRegistration.Application.Services;
using PersonRegistration.Application.Validators;
using PersonRegistration.ConsoleApp;
using PersonRegistration.Domain.Interfaces;
using PersonRegistration.Domain.Validators;

var builder = Host.CreateApplicationBuilder(args);

// Configure logging (console) and add configuration (appsettings.json is auto-loaded)
builder.Services.AddLogging(config => {
    config.AddConsole();
    config.SetMinimumLevel(LogLevel.Information);
});

IConfiguration config = builder.Configuration;  



// Register services and abstractions
builder.Services.AddSingleton<IConfiguration>(config);
builder.Services.AddSingleton<IUserInteractionService, ConsoleUserInteractionService>();
builder.Services.AddSingleton<IPersonRepository, FilePersonRepository>();
builder.Services.AddSingleton<IPersonValidator, PersonValidator>();
builder.Services.AddSingleton<PersonRegistrationService>();
// Add the hosted service that drives the app
builder.Services.AddHostedService<PersonRegistrationHostedService>();

var host = builder.Build();
await host.RunAsync();