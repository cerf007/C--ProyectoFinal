using Microsoft.Extensions.Logging;
using SQLite;


namespace MauiApp1
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
            string dbDireccion = Path.Combine(FileSystem.AppDataDirectory, "ProyectoFinal.db3");

            // Registra la conexión asíncrona de SQLite como un Singleton
            builder.Services.AddSingleton(s => new SQLiteAsyncConnection(dbDireccion));

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
