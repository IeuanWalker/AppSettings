using BlazorProject;
using BlazorProject.Components;
using FluentValidation;
using IeuanWalker.AppSettings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

builder.AddAppSettingsFromBlazorProject();






builder.Services.AddOptions<global::BlazorProject.ConfirmationEmailSettings>().Configure(options => builder.Configuration.GetSection(global::BlazorProject.ConfirmationEmailSettings.SectionName ?? typeof(global::BlazorProject.ConfirmationEmailSettings).Name).Bind(options));

builder.Services.AddOptions<global::BlazorProject.ConfirmationEmailSettings1>().Configure(options => builder.Configuration.GetSection(global::BlazorProject.ConfirmationEmailSettings1.SectionName ?? typeof(global::BlazorProject.ConfirmationEmailSettings1).Name).Bind(options));

builder.Services.AddScoped<IValidator<global::BlazorProject.ClosureEmailSettings>, global::BlazorProject.ClosureEmailSettingsValidator>();
builder.Services.AddOptions<global::BlazorProject.ClosureEmailSettings>()
	.Configure(options => builder.Configuration.GetSection(global::BlazorProject.ClosureEmailSettings.SectionName ?? typeof(global::BlazorProject.ClosureEmailSettings).Name).Bind(options))
	.ValidateFluentValidation()
	.ValidateOnStart();









var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();
