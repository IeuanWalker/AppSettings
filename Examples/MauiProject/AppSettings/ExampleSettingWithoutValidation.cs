using IeuanWalker.AppSettings;

namespace MauiProject.AppSettings;

public class ExampleSettingWithoutValidation : IAppSettings
{
	public static string? SectionName => "ExampleSetting";
	public required string Test { get; set; }
}
