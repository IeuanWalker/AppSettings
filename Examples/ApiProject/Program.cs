using ApiProject;
using ApiProjectNestedClassLibrary;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.AddAppSettingsFromApiProject();
builder.Services.AddAppSettingsFromApiProjectNestedClassLibrary(builder.Configuration);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapGet("/test", (
	IOptions<ConfirmationEmailSettings> confirmationEmailSettings,
	IOptions<ConfirmationEmailSettings1> confirmationEmailSettings1,
	IOptions<ConfirmationEmailSettingsFromAttribute> confirmationEmailSettingsFromAttribute,
	IOptions<ClosureEmailSettings> closureEmailSettings,
	IOptions<ClosureEmailSettingsFromAttribute> closureEmailSettingsFromAttribute,
	IOptions<NestedClassLibraryAppSetting> nestedClassLibraryAppSetting,
	IOptions<NestedClassLibraryAppSettingFromAttribute> nestedClassLibraryAppSettingFromAttribute) =>
{
	return $$"""
	Subject from ConfirmationEmailSettings: {{confirmationEmailSettings.Value.Subject}}
	Subject from ConfirmationEmailSettings1: {{confirmationEmailSettings1.Value.Subject}}
	Subject from ConfirmationEmailSettingsAttribute: {{confirmationEmailSettingsFromAttribute.Value.Subject}}
	Subject from ClosureEmailSettings: {{closureEmailSettings.Value.Subject}}
	Subject from ClosureEmailSettingsFromAttribute: {{closureEmailSettingsFromAttribute.Value.Subject}}
	Text from NestedClassLibraryAppSetting: {{nestedClassLibraryAppSetting.Value.Test}}
	Text from NestedClassLibraryAppSettingFromAttribute: {{nestedClassLibraryAppSettingFromAttribute.Value.Test}}
	""";
});


await app.RunAsync();
