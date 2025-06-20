using System.Text;
using Microsoft.Extensions.Options;
using MuaiProject;

namespace MauiProject;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();

		IOptions<ConfirmationEmailSettings>? confirmationEmailSettings = Application.Current?.Windows.FirstOrDefault()?.Handler?.MauiContext?.Services.GetRequiredService<IOptions<ConfirmationEmailSettings>>();
		IOptions<ConfirmationEmailSettings1>? confirmationEmailSettings1 = Application.Current?.Windows.FirstOrDefault()?.Handler?.MauiContext?.Services.GetRequiredService<IOptions<ConfirmationEmailSettings1>>();
		IOptions<ClosureEmailSettings>? closureEmailSettings = Application.Current?.Windows.FirstOrDefault()?.Handler?.MauiContext?.Services.GetRequiredService<IOptions<ClosureEmailSettings>>();


		StringBuilder sb = new();

		sb.AppendLine("Subject from ConfirmationEmailSettings: ").Append(confirmationEmailSettings?.Value.Subject);
		sb.AppendLine(Environment.NewLine);
		sb.AppendLine("Subject from ConfirmationEmailSettings1: ").Append(confirmationEmailSettings1?.Value.Subject);
		sb.AppendLine(Environment.NewLine);
		sb.AppendLine("Subject from ClosureEmailSettings: ").Append(closureEmailSettings?.Value.Subject);

		Content = new Label()
		{
			Text = sb.ToString(),
		};
	}
}