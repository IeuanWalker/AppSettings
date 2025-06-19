using IeuanWalker.AppSettings;

namespace NestedProject;

public class ExampleSettingWithoutValidation : IAppSettings
{
	public static string? SectionName => "ExampleSetting";
	public required string Test { get; set; }
}