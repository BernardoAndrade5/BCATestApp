using Microsoft.Extensions.Logging;
using BCATestApp.Services;

using BCATestApp.ViewModel;
using BCATestApp.Repositorys;
using CommunityToolkit.Maui;

namespace BCATestApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddSingleton<CarService>();
            builder.Services.AddSingleton<NavigationService>();
            builder.Services.AddSingleton<CarsViewModel>();
            builder.Services.AddTransient<CarsRepository>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
