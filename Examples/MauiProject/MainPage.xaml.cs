using Microsoft.Extensions.Options;

namespace MauiProject;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();

		IOptions<DataAnnotationWithNoValidationOrSectionNameSettings> _dataAnnotationWithNoValidationOrSectionNameSettings = Application.Current?.Windows.FirstOrDefault()?.Handler?.MauiContext?.Services.GetRequiredService<IOptions<DataAnnotationWithNoValidationOrSectionNameSettings>>() ?? throw new NullReferenceException();
		IOptions<DataAnnotationWithNoValidationButWithSectionNameSettings> _dataAnnotationWithNoValidationButWithSectionNameSettings = Application.Current?.Windows.FirstOrDefault()?.Handler?.MauiContext?.Services.GetRequiredService<IOptions<DataAnnotationWithNoValidationButWithSectionNameSettings>>() ?? throw new NullReferenceException();
		IOptions<DataAnnotationWithValidationButNoSectionNameSettings> _dataAnnotationWithValidationButNoSectionNameSettings = Application.Current?.Windows.FirstOrDefault()?.Handler?.MauiContext?.Services.GetRequiredService<IOptions<DataAnnotationWithValidationButNoSectionNameSettings>>() ?? throw new NullReferenceException();
		IOptions<DataAnnotationWithValidationAndSectionNameSettings> _dataAnnotationWithValidationAndSectionNameSettings = Application.Current?.Windows.FirstOrDefault()?.Handler?.MauiContext?.Services.GetRequiredService<IOptions<DataAnnotationWithValidationAndSectionNameSettings>>() ?? throw new NullReferenceException();
		IOptions<DataAnnotationNestedSettings> _dataAnnotationNestedSettings = Application.Current?.Windows.FirstOrDefault()?.Handler?.MauiContext?.Services.GetRequiredService<IOptions<DataAnnotationNestedSettings>>() ?? throw new NullReferenceException();

		IOptions<FluentValidationWithNoValidationOrSectionNameSettings> _fluentValidationWithNoValidationOrSectionNameSettings = Application.Current?.Windows.FirstOrDefault()?.Handler?.MauiContext?.Services.GetRequiredService<IOptions<FluentValidationWithNoValidationOrSectionNameSettings>>() ?? throw new NullReferenceException();
		IOptions<FluentValidationWithNoValidationButWithSectionNameSettings> _fluentValidationWithNoValidationButWithSectionNameSettings = Application.Current?.Windows.FirstOrDefault()?.Handler?.MauiContext?.Services.GetRequiredService<IOptions<FluentValidationWithNoValidationButWithSectionNameSettings>>() ?? throw new NullReferenceException();
		IOptions<FluentValidationWithValidationButNoSectionNameSettings> _fluentValidationWithValidationButNoSectionNameSettings = Application.Current?.Windows.FirstOrDefault()?.Handler?.MauiContext?.Services.GetRequiredService<IOptions<FluentValidationWithValidationButNoSectionNameSettings>>() ?? throw new NullReferenceException();
		IOptions<FluentValidationWithValidationAndSectionNameSettings> _fluentValidationWithValidationAndSectionNameSettings = Application.Current?.Windows.FirstOrDefault()?.Handler?.MauiContext?.Services.GetRequiredService<IOptions<FluentValidationWithValidationAndSectionNameSettings>>() ?? throw new NullReferenceException();
		IOptions<FluentValidationNestedSettings> _fluentValidationNestedSettings = Application.Current?.Windows.FirstOrDefault()?.Handler?.MauiContext?.Services.GetRequiredService<IOptions<FluentValidationNestedSettings>>() ?? throw new NullReferenceException();

		Content = new Label()
		{
			Text = $"""
   DataAnnotation
   Text from DataAnnotationWithNoValidationOrSectionNameSettings: {_dataAnnotationWithNoValidationOrSectionNameSettings.Value.Text}
   Text from DataAnnotationWithNoValidationButWithSectionNameSettings: {_dataAnnotationWithNoValidationButWithSectionNameSettings.Value.Text}
   Text from DataAnnotationWithValidationButNoSectionNameSettings: {_dataAnnotationWithValidationButNoSectionNameSettings.Value.Text}
   Text from DataAnnotationWithValidationAndSectionNameSettings: {_dataAnnotationWithValidationAndSectionNameSettings.Value.Text}
   Text from DataAnnotationNestedSettings: {_dataAnnotationNestedSettings.Value.Text}

   FluentValidation
   Text from FluentValidationWithNoValidationOrSectionNameSettings: {_fluentValidationWithNoValidationOrSectionNameSettings.Value.Text}
   Text from FluentValidationWithNoValidationButWithSectionNameSettings: {_fluentValidationWithNoValidationButWithSectionNameSettings.Value.Text}
   Text from FluentValidationWithValidationButNoSectionNameSettings: {_fluentValidationWithValidationButNoSectionNameSettings.Value.Text}
   Text from FluentValidationWithValidationAndSectionNameSettings: {_fluentValidationWithValidationAndSectionNameSettings.Value.Text}
   Text from FluentValidationNestedSettings: {_fluentValidationNestedSettings.Value.Text}
   """
		};
	}
}