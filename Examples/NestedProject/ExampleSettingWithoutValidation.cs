using IeuanWalker.AppSettings;

namespace ApiProjectNestedClassLibrary;

public class ExampleSettingWithoutValidation : IAppSettings
{
	public static string? SectionName => "ExampleSetting";
	public required string Test { get; set; }
}