using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IeuanWalker.AppSettings;

static class OptionsBuilderFluentValidationExtensions
{
	public static OptionsBuilder<TOptions> ValidateFluentValidation<TOptions>(this OptionsBuilder<TOptions> optionsBuilder) where TOptions : class
	{
		optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(provider => new FluentValidationOptions<TOptions>(optionsBuilder.Name, provider));
		return optionsBuilder;
	}
}

sealed class FluentValidationOptions<TOptions> : IValidateOptions<TOptions> where TOptions : class
{
	readonly IServiceProvider _serviceProvider;
	readonly string? _name;

	public FluentValidationOptions(string? name, IServiceProvider serviceProvider)
	{
		// we need the service provider to create a scope later
		_serviceProvider = serviceProvider;
		_name = name; // Handle named options
	}

	public ValidateOptionsResult Validate(string? name, TOptions options)
	{
		// Null name is used to configure all named options.
		if(_name is not null && _name != name)
		{
			// Ignored if not validating this instance.
			return ValidateOptionsResult.Skip;
		}

		// Ensure options are provided to validate against
		ArgumentNullException.ThrowIfNull(options);

		// Validators are typically registered as scoped,
		// so we need to create a scope to be safe, as this
		// method is be called from the root scope
		using IServiceScope scope = _serviceProvider.CreateScope();

		// retrieve an instance of the validator
		IValidator<TOptions> validator = scope.ServiceProvider.GetRequiredService<IValidator<TOptions>>();

		// Run the validation
		ValidationResult results = validator.Validate(options);
		if(results.IsValid)
		{
			// All good!
			return ValidateOptionsResult.Success;
		}

		// Validation failed, so build the error message
		string typeName = options.GetType().Name;
		List<string> errors = [];
		foreach(ValidationFailure? result in results.Errors)
		{
			errors.Add($"Fluent validation failed for '{typeName}.{result.PropertyName}' with the error: '{result.ErrorMessage}'.");
		}

		return ValidateOptionsResult.Fail(errors);
	}
}