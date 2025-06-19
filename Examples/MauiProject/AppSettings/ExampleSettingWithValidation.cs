using FluentValidation;
using IeuanWalker.AppSettings;

namespace MauiProject.AppSettings;

public class ExampleSettingWithValidation : IAppSettings<ExampleSettingWithValidationValidator>
{
	public static string? SectionName => "ExampleSetting";
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