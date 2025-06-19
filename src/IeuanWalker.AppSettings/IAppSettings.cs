using FluentValidation;

namespace IeuanWalker.AppSettings;
public interface IAppSettings
{
	static virtual string? SectionName => null;
}

public interface IAppSettings<TValidator> where TValidator : class, IValidator
{
	static virtual string? SectionName => null;
}
