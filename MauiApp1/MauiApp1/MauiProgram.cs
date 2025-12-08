using MauiApp1.Pages.Sesiones;
using MauiApp1.Servicios;
using MauiApp1.ViewModels;
using MauiApp1.ViewModels.SesionesVM;
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
            // Registro de Servicios y ViewModels para Inyección de Dependencias
            builder.Services.AddSingleton<DatabaseService>(); // Contiene la BD y Super Admin
            builder.Services.AddSingleton<AutenticacionServicio>(); // Contiene la lógica de login

            builder.Services.AddTransient<MainPageVM>(); // El ViewModel de tu página de Login
            builder.Services.AddTransient<MainPage>(); // Tu página de Login

            builder.Services.AddTransient<SesionGerenteVM>(); // ViewModel para la sesión de Gerente
            builder.Services.AddTransient<SesionGerente>(); // Página para la sesión de Gerente

            builder.Services.AddTransient<SesionEmpleadoVM>(); // ViewModel para la sesión de Personal Básico
            builder.Services.AddTransient<SesionEmpleado>(); // Página para la sesión de Personal Básico

            return builder.Build();
        }
    }
}
