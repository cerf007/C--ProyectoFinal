using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.Modelos;
using MauiApp1.Pages.Sesiones; // Necesario para usar nameof(SesionGerente) y otros
using MauiApp1.Servicios;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace MauiApp1.ViewModels
{
    // Clase principal del ViewModel para la página de Login
    public partial class MainPageVM : ObservableObject
    {
        // El servicio de autenticación inyectado
        private readonly AutenticacionServicio _autenticacionServicio;

        // Constructor que recibe el servicio por Inyección de Dependencias
        public MainPageVM(AutenticacionServicio autenticacionServicio)
        {
            _autenticacionServicio = autenticacionServicio;
        }

        [ObservableProperty]
        private string usuarioInput;

        [ObservableProperty]
        private string contrasenaInput;

        // Comando que se ejecuta al presionar el botón "Iniciar Sesión"
        [RelayCommand]
        public async Task IniciarSesion()
        {
            // 1. Validación de campos vacíos
            if (string.IsNullOrWhiteSpace(UsuarioInput) || string.IsNullOrWhiteSpace(ContrasenaInput))
            {
                await Shell.Current.DisplayAlert("Error", "Debes ingresar tu usuario y contraseña.", "OK");
                return;
            }

            // 2. Llamada a la lógica de autenticación (BD)
            var personal = await _autenticacionServicio.AuthenticateUserAsync(UsuarioInput, ContrasenaInput);

            if (personal != null)
            {
                // AUTENTICACIÓN EXITOSA

                // Guardar rol con Preferences (MAUI)
                try
                {
                    Preferences.Set("CurrentUserCargo", personal.Cargo ?? string.Empty);
                }
                catch
                {
                    // Si falla el guardado, no rompes la navegación — seguimos adelante
                }

                // Limpiar campos después del login
                UsuarioInput = string.Empty;
                ContrasenaInput = string.Empty;

                string rutaDestino;

                // 3. Determinar la navegación por el Cargo/Rol (sin case sensitivity)
                if (!string.IsNullOrWhiteSpace(personal.Cargo) && personal.Cargo.Equals("Gerente", System.StringComparison.OrdinalIgnoreCase))
                {
                    rutaDestino = nameof(SesionGerente);
                }
                else
                {
                    // Cualquier otro cargo (Administrador, Operario, Vendedor, etc.) -> sesión de empleado
                    rutaDestino = nameof(SesionEmpleado);
                }

                // 4. Navegación relativa (evita el error de rutas globales como única página)
                try
                {
                    await Shell.Current.GoToAsync(rutaDestino);
                }
                catch (System.Exception ex)
                {
                    // Fallback: intentamos navegar de forma absoluta si la relativa falla (y mostramos razón)
                    await Shell.Current.DisplayAlert("Error", $"No se pudo navegar: {ex.Message}. Intentando navegación alternativa...", "OK");
                    try
                    {
                        // Alternativa: volver al root y navegar (opcional)
                        await Shell.Current.Navigation.PopToRootAsync();
                        await Shell.Current.GoToAsync(rutaDestino);
                    }
                    catch (System.Exception ex2)
                    {
                        await Shell.Current.DisplayAlert("Error", $"No se pudo navegar (alternativa fallida): {ex2.Message}", "OK");
                    }
                }
            }
            else
            {
                // AUTENTICACIÓN FALLIDA
                await Shell.Current.DisplayAlert("Error", "Credenciales incorrectas. Intenta de nuevo.", "Reintentar");
            }
        }
    }
}