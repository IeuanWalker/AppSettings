using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace BlazorProject.Components.Pages;
public partial class Home
{
	[Inject] IOptions<ConfirmationEmailSettings> _confirmationEmailSettings { get; set; } = default!;
	[Inject] IOptions<ConfirmationEmailSettings1> _confirmationEmailSettings1 { get; set; } = default!;
	[Inject] IOptions<ConfirmationEmailSettingsFromAttribute> _confirmationEmailSettingsFromAttribute { get; set; } = default!;
	[Inject] IOptions<ClosureEmailSettings> _closureEmailSettings { get; set; } = default!;
	[Inject] IOptions<ClosureEmailSettingsFromAttribute> _closureEmailSettingsFromAttribute { get; set; } = default!;
}