using IeuanWalker.AppSettings;

namespace ApiProjectNestedClassLibrary;

public record NestedClassLibraryAppSetting : IAppSettings
{
	public required string Test { get; set; }
}

[SectionName("NestedClassLibraryAppSetting")]
public record NestedClassLibraryAppSettingFromAttribute : IAppSettings
{
	public required string Test { get; set; }
}
