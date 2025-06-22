using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace BlazorProject.Components.Pages;
public partial class Home
{
	// Data Annotations
	[Inject] IOptions<DataAnnotationWithNoValidationOrSectionNameSettings> _dataAnnotationWithNoValidationOrSectionNameSettings { get; set; } = default!;
	[Inject] IOptions<DataAnnotationWithNoValidationButWithSectionNameSettings> _dataAnnotationWithNoValidationButWithSectionNameSettings { get; set; } = default!;
	[Inject] IOptions<DataAnnotationWithValidationButNoSectionNameSettings> _dataAnnotationWithValidationButNoSectionNameSettings { get; set; } = default!;
	[Inject] IOptions<DataAnnotationWithValidationAndSectionNameSettings> _dataAnnotationWithValidationAndSectionNameSettings { get; set; } = default!;
	[Inject] IOptions<DataAnnotationNestedSettings> _dataAnnotationNestedSettings { get; set; } = default!;

	// Fluent Validation
	[Inject] IOptions<FluentValidationWithNoValidationOrSectionNameSettings> _fluentValidationWithNoValidationOrSectionNameSettings { get; set; } = default!;
	[Inject] IOptions<FluentValidationWithNoValidationButWithSectionNameSettings> _fluentValidationWithNoValidationButWithSectionNameSettings { get; set; } = default!;
	[Inject] IOptions<FluentValidationWithValidationButNoSectionNameSettings> _fluentValidationWithValidationButNoSectionNameSettings { get; set; } = default!;
	[Inject] IOptions<FluentValidationWithValidationAndSectionNameSettings> _fluentValidationWithValidationAndSectionNameSettings { get; set; } = default!;
	[Inject] IOptions<FluentValidationNestedSettings> _fluentValidationNestedSettings { get; set; } = default!;
	
	[Inject] IOptions<ConfirmationEmailSettingsFromAttribute> _confirmationEmailSettingsFromAttribute { get; set; } = default!;
	[Inject] IOptions<ClosureEmailSettingsFromAttribute> _closureEmailSettingsFromAttribute { get; set; } = default!;
}