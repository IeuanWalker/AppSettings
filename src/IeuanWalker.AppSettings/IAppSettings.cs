using FluentValidation;

namespace IeuanWalker.AppSettings;
public interface IAppSettings
{
}

public interface IAppSettings<TValidator> where TValidator : class, IValidator
{
}
