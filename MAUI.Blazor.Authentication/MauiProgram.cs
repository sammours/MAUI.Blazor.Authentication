using MAUI.Blazor.Authentication.Clients;
using MAUI.Blazor.Authentication.Models;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace MAUI.Blazor.Authentication;

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
			});

		builder.Services.AddMauiBlazorWebView();
#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
#endif

        var a = Assembly.GetExecutingAssembly();
        using var stream = a.GetManifestResourceStream("MAUI.Blazor.Authentication.appsettings.json");

        var config = new ConfigurationBuilder()
                    .AddJsonStream(stream)
                    .Build();

        builder.Configuration.AddConfiguration(config);

        var configuration = builder.Configuration;
        var azureAdAuthenticationConfiguration = configuration.GetRequiredSection("AzureAdConfiguration").Get<AzureAdConfiguration>();

        builder.Services
            .AddSingleton<IPublicAuthenticationClient>(new PublicAuthenticationClient(azureAdAuthenticationConfiguration));

        builder.Services
            .AddSingleton<IMachineAuthenticationClient>(new MachineAuthenticationClient(azureAdAuthenticationConfiguration));

        return builder.Build();
	}
}
