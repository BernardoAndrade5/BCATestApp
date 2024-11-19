using Microsoft.Extensions.Logging;
using BCATestApp.Services;

using BCATestApp.ViewModel;
using BCATestApp.Repositorys;

namespace BCATestApp
{
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

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddSingleton<CarService>();
            builder.Services.AddTransient<CarsViewModel>();
            builder.Services.AddTransient<CarsRepository>();


#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
