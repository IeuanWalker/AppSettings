using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MauiProject;
public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MauiProject.appsettings.json")!;
		IConfigurationRoot config = new ConfigurationBuilder()
			.AddJsonStream(stream)
			.Build();
		builder.Configuration.AddConfiguration(config);

		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.AddAppSettingsFromMauiProject();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
