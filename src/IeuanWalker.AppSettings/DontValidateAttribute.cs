namespace IeuanWalker.AppSettings;

/// <summary>
/// Use this attribute to prevent validating classes that implement the IAppSetting interface.
/// When applied, the class will be excluded from the validation process.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class DontValidateAttribute : Attribute
{
}