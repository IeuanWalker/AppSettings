using ConsoleProject;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

IConfigurationBuilder builder = new ConfigurationBuilder()
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile($"appsettings.json", optional: false, reloadOnChange: false)
	.AddEnvironmentVariables();

IConfigurationRoot configuration = builder.Build();

IServiceCollection serviceCollection = new ServiceCollection();
serviceCollection.AddOptions();
serviceCollection.AddAppSettingsFromConsoleProject(configuration);

ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

IOptions<ConfirmationEmailSettings> confirmationEmailSettings = serviceProvider.GetService<IOptions<ConfirmationEmailSettings>>() ?? throw new NullReferenceException();
IOptions<ConfirmationEmailSettings1> confirmationEmailSettings1 = serviceProvider.GetService<IOptions<ConfirmationEmailSettings1>>() ?? throw new NullReferenceException();
IOptions<ClosureEmailSettings> closureEmailSettings = serviceProvider.GetService<IOptions<ClosureEmailSettings>>() ?? throw new NullReferenceException();


Console.WriteLine($"""
Subject from ConfirmationEmailSettings: {confirmationEmailSettings.Value.Subject}
Subject from ConfirmationEmailSettings1: {confirmationEmailSettings1.Value.Subject}
Subject from ClosureEmailSettings: {closureEmailSettings.Value.Subject}
""");

Console.ReadLine();
