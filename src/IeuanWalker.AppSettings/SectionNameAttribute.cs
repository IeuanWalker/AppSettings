namespace IeuanWalker.AppSettings;

/// <summary>
/// Use this attribute when the configuration section name differs from the class name.
/// </summary>
/// <param name="sectionName">The name of the section</param>
/// <remarks>
/// <para>
/// Supports hierarchical paths using the colon separator.
/// </para>
/// [SectionName("Section:ChildSection")]
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public class SectionNameAttribute(string sectionName) : Attribute
{
	public string SectionName { get; } = sectionName;
}