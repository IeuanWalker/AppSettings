using FluentValidation;
using IeuanWalker.AppSettings;

namespace NestedProject;

public class ExampleSettingWithValidation : IAppSettings
{
	public required string Test { get; set; }
}

sealed class ExampleSettingWithValidationValidator : AbstractValidator<ExampleSettingWithValidation>
{
	public ExampleSettingWithValidationValidator()
	{
		RuleFor(x => x.Test)
			.NotEmpty()
			.MinimumLength(5);
	}
}