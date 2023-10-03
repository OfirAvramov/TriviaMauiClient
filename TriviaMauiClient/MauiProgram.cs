
using Microsoft.Extensions.Logging;
using TriviaMauiClient.Services;
using TriviaMauiClient.ViewModels;
using TriviaMauiClient.Views;

namespace TriviaMauiClient;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
        builder.Services.AddSingleton<TriviaService>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<MainPageViewModel>();
		builder.Services.AddTransient<LoadingPage>();
		builder.Services.AddSingleton<GamePage>();
		builder.Services.AddSingleton<GamePageViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
