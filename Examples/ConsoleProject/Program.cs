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

IOptions<DataAnnotationWithNoValidationOrSectionNameSettings> _dataAnnotationWithNoValidationOrSectionNameSettings = serviceProvider.GetService<IOptions<DataAnnotationWithNoValidationOrSectionNameSettings>>() ?? throw new NullReferenceException();
IOptions<DataAnnotationWithNoValidationButWithSectionNameSettings> _dataAnnotationWithNoValidationButWithSectionNameSettings = serviceProvider.GetService<IOptions<DataAnnotationWithNoValidationButWithSectionNameSettings>>() ?? throw new NullReferenceException();
IOptions<DataAnnotationWithValidationButNoSectionNameSettings> _dataAnnotationWithValidationButNoSectionNameSettings = serviceProvider.GetService<IOptions<DataAnnotationWithValidationButNoSectionNameSettings>>() ?? throw new NullReferenceException();
IOptions<DataAnnotationWithValidationAndSectionNameSettings> _dataAnnotationWithValidationAndSectionNameSettings = serviceProvider.GetService<IOptions<DataAnnotationWithValidationAndSectionNameSettings>>() ?? throw new NullReferenceException();
IOptions<DataAnnotationNestedSettings> _dataAnnotationNestedSettings = serviceProvider.GetService<IOptions<DataAnnotationNestedSettings>>() ?? throw new NullReferenceException();

IOptions<FluentValidationWithNoValidationOrSectionNameSettings> _fluentValidationWithNoValidationOrSectionNameSettings = serviceProvider.GetService<IOptions<FluentValidationWithNoValidationOrSectionNameSettings>>() ?? throw new NullReferenceException();
IOptions<FluentValidationWithNoValidationButWithSectionNameSettings> _fluentValidationWithNoValidationButWithSectionNameSettings = serviceProvider.GetService<IOptions<FluentValidationWithNoValidationButWithSectionNameSettings>>() ?? throw new NullReferenceException();
IOptions<FluentValidationWithValidationButNoSectionNameSettings> _fluentValidationWithValidationButNoSectionNameSettings = serviceProvider.GetService<IOptions<FluentValidationWithValidationButNoSectionNameSettings>>() ?? throw new NullReferenceException();
IOptions<FluentValidationWithValidationAndSectionNameSettings> _fluentValidationWithValidationAndSectionNameSettings = serviceProvider.GetService<IOptions<FluentValidationWithValidationAndSectionNameSettings>>() ?? throw new NullReferenceException();
IOptions<FluentValidationNestedSettings> _fluentValidationNestedSettings = serviceProvider.GetService<IOptions<FluentValidationNestedSettings>>() ?? throw new NullReferenceException();


Console.WriteLine($"""
DataAnnotation
Text from DataAnnotationWithNoValidationOrSectionNameSettings: {_dataAnnotationWithNoValidationOrSectionNameSettings.Value.Text}
Text from DataAnnotationWithNoValidationButWithSectionNameSettings: {_dataAnnotationWithNoValidationButWithSectionNameSettings.Value.Text}
Text from DataAnnotationWithValidationButNoSectionNameSettings: {_dataAnnotationWithValidationButNoSectionNameSettings.Value.Text}
Text from DataAnnotationWithValidationAndSectionNameSettings: {_dataAnnotationWithValidationAndSectionNameSettings.Value.Text}
Text from DataAnnotationNestedSettings: {_dataAnnotationNestedSettings.Value.Text}

FluentValidation
Text from FluentValidationWithNoValidationOrSectionNameSettings: {_fluentValidationWithNoValidationOrSectionNameSettings.Value.Text}
Text from FluentValidationWithNoValidationButWithSectionNameSettings: {_fluentValidationWithNoValidationButWithSectionNameSettings.Value.Text}
Text from FluentValidationWithValidationButNoSectionNameSettings: {_fluentValidationWithValidationButNoSectionNameSettings.Value.Text}
Text from FluentValidationWithValidationAndSectionNameSettings: {_fluentValidationWithValidationAndSectionNameSettings.Value.Text}
Text from FluentValidationNestedSettings: {_fluentValidationNestedSettings.Value.Text}
""");

Console.ReadLine();
