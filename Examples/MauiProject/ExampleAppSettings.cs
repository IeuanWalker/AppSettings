using System.ComponentModel.DataAnnotations;
using FluentValidation;
using IeuanWalker.AppSettings;

namespace MauiProject;

#region DataAnnotations

public class DataAnnotationWithNoValidationOrSectionNameSettings : IAppSettings
{
	public required string Text { get; set; }
}
public class DataAnnotationWithNoValidationButWithSectionNameSettings : IAppSettings
{
	public static string? SectionName => "DataAnnotationWithNoValidationButWithSectionName";
	public required string Text { get; set; }
}
public class DataAnnotationWithValidationButNoSectionNameSettings : IAppSettings
{
	[MinLength(5)]
	public required string Text { get; set; }
}

public class DataAnnotationWithValidationAndSectionNameSettings : IAppSettings
{
	public static string? SectionName => "DataAnnotationWithValidationAndSectionName";
	[MinLength(5)]
	public required string Text { get; set; }
}

public class DataAnnotationNestedSettings : IAppSettings
{
	public static string? SectionName => "DataAnnotationNestedSettings:Settings";
	[MinLength(5)]
	public required string Text { get; set; }
}

#endregion

#region FluentValidation

public class FluentValidationWithNoValidationOrSectionNameSettings : IAppSettings
{
	public required string Text { get; set; }
}
public class FluentValidationWithNoValidationButWithSectionNameSettings : IAppSettings
{
	public static string? SectionName => "FluentValidationWithNoValidationButWithSectionName";
	public required string Text { get; set; }
}
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

public class FluentValidationWithValidationAndSectionNameSettings : IAppSettings<FluentValidationWithValidationAndSectionNameSettingValidator>
{
	public static string? SectionName => "FluentValidationWithValidationAndSectionName";
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

public class FluentValidationNestedSettings : IAppSettings<FluentValidationNestedSettingsValidator>
{
	public static string? SectionName => "FluentValidationNestedSettings:Settings";
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