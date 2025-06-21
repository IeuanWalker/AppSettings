namespace IeuanWalker.AppSettings;

/// <summary>
/// Specify the appsettings section if it is different from the class name
/// </summary>
/// <param name="sectionName">The name of the section</param>
/// <example>[SectionName("Section:ChildSection"]</example>
[AttributeUsage(AttributeTargets.Class)]
public class SectionNameAttribute(string sectionName) : Attribute
{
	public string SectionName { get; } = sectionName;
}