using IeuanWalker.AppSettings;

namespace MuaiProject;

public class ConfirmationEmailSettings : IAppSettings
{
	public required string Subject { get; set; }
}
public class ConfirmationEmailSettings1 : IAppSettings
{
	public static string? SectionName => "ConfirmationEmail";
	public required string Subject { get; set; }
}

public class ClosureEmailSettings : IAppSettings
{
	public static string? SectionName => "NestedObject:ClosureEmail";
	public required string Subject { get; set; }
}