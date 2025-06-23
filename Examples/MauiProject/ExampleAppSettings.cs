using System.ComponentModel.DataAnnotations;
using FluentValidation;
using IeuanWalker.AppSettings;

namespace MauiProject;

#region DataAnnotations

public class DataAnnotationWithNoValidationOrSectionNameSettings : IAppSettings
{
	public required string Text { get; set; }
}
[SectionName("DataAnnotationWithNoValidationButWithSectionName")]
public class DataAnnotationWithNoValidationButWithSectionNameSettings : IAppSettings
{
	public required string Text { get; set; }
}
[DontValidate]
public class DataAnnotationWithValidationButNoSectionNameSettings : IAppSettings
{
	[MinLength(5)]
	public required string Text { get; set; }
}

[SectionName("DataAnnotationWithValidationAndSectionName")]
public class DataAnnotationWithValidationAndSectionNameSettings : IAppSettings
{
	[MinLength(5)]
	public required string Text { get; set; }
}

[SectionName("DataAnnotationNestedSettings:Settings")]
public class DataAnnotationNestedSettings : IAppSettings
{
	[MinLength(5)]
	public required string Text { get; set; }
}

#endregion

#region FluentValidation

public class FluentValidationWithNoValidationOrSectionNameSettings : IAppSettings
{
	public required string Text { get; set; }
}

[SectionName("FluentValidationWithNoValidationButWithSectionName")]
public class FluentValidationWithNoValidationButWithSectionNameSettings : IAppSettings
{
	public required string Text { get; set; }
}

[DontValidate]
public class FluentValidationWithValidationButNoSectionNameSettings : IAppSettings<FluentValidationWithValidationButNoSectionNameSettingsValidator>
{
	public required string Text { get; set; }
}

sealed class FluentValidationWithValidationButNoSectionNameSettingsValidator : AbstractValidator<FluentValidationWithValidationButNoSectionNameSettings>
{
	public FluentValidationWithValidationButNoSectionNameSettingsValidator()
	{
		RuleFor(x => x.Text)
			.NotEmpty()
			.MinimumLength(5);
	}
}

[SectionName("FluentValidationWithValidationAndSectionName")]
public class FluentValidationWithValidationAndSectionNameSettings : IAppSettings<FluentValidationWithValidationAndSectionNameSettingValidator>
{
	public required string Text { get; set; }
}

sealed class FluentValidationWithValidationAndSectionNameSettingValidator : AbstractValidator<FluentValidationWithValidationAndSectionNameSettings>
{
	public FluentValidationWithValidationAndSectionNameSettingValidator()
	{
		RuleFor(x => x.Text)
			.NotEmpty()
			.MinimumLength(5);
	}
}

[SectionName("FluentValidationNestedSettings:Settings")]
public class FluentValidationNestedSettings : IAppSettings<FluentValidationNestedSettingsValidator>
{
	public required string Text { get; set; }
}

sealed class FluentValidationNestedSettingsValidator : AbstractValidator<FluentValidationNestedSettings>
{
	public FluentValidationNestedSettingsValidator()
	{
		RuleFor(x => x.Text)
			.NotEmpty()
			.MinimumLength(5);
	}
}
#endregion