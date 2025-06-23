# AppSettings [![Nuget](https://img.shields.io/nuget/v/IeuanWalker.AppSettings)](https://www.nuget.org/packages/IeuanWalker.AppSettings) [![Nuget](https://img.shields.io/nuget/dt/IeuanWalker.AppSettings)](https://www.nuget.org/packages/IeuanWalker.AppSettings) [![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

Automatically generates the registration code for IOptions and can validate them on startup using [DataAnnotations](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-9.0#options-validation) or [fluent validation](https://docs.fluentvalidation.net/en/latest/).

## How to use it?
1. Install the [NuGet package](https://www.nuget.org/packages/IeuanWalker.AppSettings) into your project.
```
Install-Package IeuanWalker.AppSettings
```

2. Inherit the `IAppSettings` interface onto the class that models your IOptions
```csharp
public class ConfirmationEmailSettings : IAppSettings
{
	public required string Subject { get; set; }
}
```
3. Register the app settings
   > Once `IAppSettings` has been added to a class in your project, several extension methods will automatically be created.
   > The extension method name convention is AddAppSettingsFrom + your assembly name, and the namespace is your assembly name.

    3.1 An extension method for `IServiceCollection` is created for all project types by default
    ```csharp
    builder.Services.AddAppSettingsFromApiProjectNestedClassLibrary(builder.Configuration);
    ```
    
    3.2 If your project is a backend/blazor project, then it will also have an extension method for `IHostApplicationBuilder`, allowing you to easily chain the registration in your progam.cs
    ```csharp
    builder.AddAppSettingsFromApiProject();
    ```
    
    3.3 If it's a MAUI project, it will also have an extension method for `MauiAppBuilder`, allowing you to easily chain the registration in your MauiProgam.cs
    ```csharp
    builder.AddAppSettingsFromMauiProject();
    ```

## Section name
By default, it maps the IOptions model to the section name based on the name of the model.
For example, the following model -
```csharp
public class ConfirmationEmailSettings : IAppSettings
{
	public required string Subject { get; set; }
}
```

Would automatically map to the following app setting section -
```json
{
  "ConfirmationEmailSettings": {
    "Subject": "Test subject"
  }
}
```

If your model name and configuration section don't match or you want to bind a nested configuration, you can override this within your model by using the `SectionName` attribute
```csharp
[SectionName("ConfirmationEmail")]
public class ConfirmationEmailSettings : IAppSettings
{
    public required string Subject { get; set; }
}
```
```csharp
[SectionName("NestedConfiguration:ConfirmationEmail")]
public class ConfirmationEmailSettings : IAppSettings
{
    public required string Subject { get; set; }
}
```

## Validation
You can perform validation on startup using DataAnnotations or FluentValidation.

### DataAnnotation
All you need to do is add a DataAnnotation attribute onto any property
```csharp
public class ConfirmationEmailSettings : IAppSettings<ConfirmationEmailSettingsValidator>
{
	[MinLength(5)]
	public required string Subject { get; set; }
}
```

### FluentValidation
To use FluentValidation you need to create an `AbstractValidator` for your app settings model and add that validator to the `IAppSettings` inheritance.
```csharp
public class ConfirmationEmailSettings : IAppSettings<ConfirmationEmailSettingsValidator>
{
	public required string Subject { get; set; }
}

sealed class ConfirmationEmailSettingsValidator : AbstractValidator<ConfirmationEmailSettings>
{
	public ConfirmationEmailSettingsValidator()
	{
		RuleFor(x => x.Subject)
			.NotEmpty()
			.MinimumLength(5);
	}
}
```

## Use FluentValidation without the source generator
You can use FluentValidation without the source generator by not inheriting from `IAppSettings` and using the extension method

```csharp
public class ConfirmationEmailSettings
{
	public required string Subject { get; set; }
}

sealed class ConfirmationEmailSettingsValidator : AbstractValidator<ConfirmationEmailSettings>
{
	public ConfirmationEmailSettingsValidator()
	{
		RuleFor(x => x.Subject)
			.NotEmpty()
			.MinimumLength(5);
	}
}
```
In your startup -
```csharp
services.AddScoped<IValidator<ConfirmationEmailSettings>, ConfirmationEmailSettingsValidator>();
services.AddOptions<ConfirmationEmailSettings>()
    .Configure(options => configuration.GetSection("FluentValidationWithValidationButNoSectionNameSettings").Bind(options))
    .ValidateFluentValidation()
    .ValidateOnStart();
```

# What does the error look like?
If something fails validation as the application starts up, you will get an exception explaining the exact issue - 
![image](https://github.com/user-attachments/assets/27465386-3970-49f7-863b-037313f4370f)

# What does the generated code look like?
The generated code is just standard C#/ .NET APIs - 
> Left is the AppSettings model, Right is the generated code 
![image](https://github.com/user-attachments/assets/4411edfc-b9e4-4eae-9cd2-c354832965b2)


# Considerations
I do not recommend adding validation to a MAUI project as it can/ will slow startup. To prevent validation, add the `DontValidate` attribute above your class.
```
[DontValidate]
public class MobileAppSettings : IAppSettings
{
	public required string Subject { get; set; }
}
```


