using JouralAppWeb.Database;
using JouralAppWeb.Services;
using Microsoft.Extensions.Logging;
using QuestPDF.Infrastructure;

namespace JouralAppWeb
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {

            QuestPDF.Settings.License = LicenseType.Community;

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });


            builder.Services.AddMauiBlazorWebView();

            builder.Services.AddSingleton<AppDbContext>();
            builder.Services.AddSingleton<Services.Theme>();
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<JournalService>();



#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
