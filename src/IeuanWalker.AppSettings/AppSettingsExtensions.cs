using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IeuanWalker.AppSettings;
public static class AppSettingsExtensions
{
	/// <summary>
	/// Adds the app settings - without validating
	/// </summary>
	/// <typeparam name="TAppSetting">App settings model</typeparam>
	public static OptionsBuilder<TAppSetting> AddAppSettings<TAppSetting>(this IServiceCollection services, IConfiguration configuration) where TAppSetting : class, IAppSettings
	{
		string sectionName = TAppSetting.SectionName ?? typeof(TAppSetting).Name;

		return services.AddOptions<TAppSetting>()
			.Configure(options =>
			{
				configuration.GetSection(sectionName).Bind(options);
			});
	}

	/// <summary>
	/// Adds the app settings - with validation
	/// </summary>
	/// <typeparam name="TAppSetting">App settings model</typeparam>
	/// <typeparam name="TValidator">App settings fluent validator</typeparam>
	public static OptionsBuilder<TAppSetting> AddAppSettings<TAppSetting, TValidator>(this IServiceCollection services, IConfiguration configuration) where TAppSetting : class, IAppSettings<TValidator> where TValidator : class, IValidator<TAppSetting>
	{
		// Add the validator
		services.AddScoped<IValidator<TAppSetting>, TValidator>();

		string sectionName = TAppSetting.SectionName ?? typeof(TAppSetting).Name;

		return services.AddOptions<TAppSetting>()
			.Configure(options =>
			{
				configuration.GetSection(sectionName).Bind(options);
			})
			.ValidateFluentValidation()
			.ValidateOnStart();
	}
}
