using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using MauiApp1.Modelos;
using MauiApp1.Servicios;
using System;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace MauiApp1.ViewModels.CredencialesVM
{
    internal class AgregarEmpleadoVM : ObservableObject
    {
        // Campos enlazados desde la vista
        public string NombreCompleto { get => _nombreCompleto; set => SetProperty(ref _nombreCompleto, value); }
        public string Usuario { get => _usuario; set => SetProperty(ref _usuario, value); }
        public string Email { get => _email; set => SetProperty(ref _email, value); }
        public string Departamento { get => _departamento; set => SetProperty(ref _departamento, value); }
        public string Cargo { get => _cargo; set => SetProperty(ref _cargo, value); }
        public string Permisos { get => _permisos; set => SetProperty(ref _permisos, value); }
        public string Contrasena { get => _contrasena; set => SetProperty(ref _contrasena, value); }

        public IAsyncRelayCommand CrearUsuarioCommand { get; }
        public IAsyncRelayCommand CancelarCommand { get; }

        private string _nombreCompleto;
        private string _usuario;
        private string _email;
        private string _departamento;
        private string _cargo;
        private string _permisos;
        private string _contrasena;

        public AgregarEmpleadoVM()
        {
            CrearUsuarioCommand = new AsyncRelayCommand(ExecuteCrearUsuarioAsync);
            CancelarCommand = new AsyncRelayCommand(ExecuteCancelarAsync);
        }

        private bool EmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            // Regex simple y robusto para email
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        private async Task ExecuteCrearUsuarioAsync()
        {
            // Validaciones básicas y amigables
            if (string.IsNullOrWhiteSpace(NombreCompleto) || NombreCompleto.Trim().Length < 3)
            {
                await Application.Current.MainPage.DisplayAlert("Validación", "Ingrese el nombre completo (mín. 3 caracteres).", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Usuario) || Usuario.Trim().Length < 3)
            {
                await Application.Current.MainPage.DisplayAlert("Validación", "Ingrese un nombre de usuario válido (mín. 3 caracteres).", "OK");
                return;
            }

            if (!string.IsNullOrWhiteSpace(Email) && !EmailValido(Email))
            {
                await Application.Current.MainPage.DisplayAlert("Validación", "Ingrese un correo electrónico válido o deje el campo vacío.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Contrasena) || Contrasena.Length < 6)
            {
                await Application.Current.MainPage.DisplayAlert("Validación", "Ingrese una contraseña de al menos 6 caracteres.", "OK");
                return;
            }

            try
            {
                // Obtener el servicio de BD vía MauiContext.Services (compatible con MAUI runtime)
                var mauiContext = Application.Current?.Handler?.MauiContext;
                var db = mauiContext?.Services.GetService<DatabaseService>();
                if (db == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Servicio de base de datos no disponible.", "OK");
                    return;
                }

                // Comprobar si ya existe un usuario con ese nombre
                var existente = await db.GetPersonalPorUsuarioAsync(Usuario.Trim());
                if (existente != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Validación", "El nombre de usuario ya existe. Elija otro.", "OK");
                    return;
                }

                // (Opcional) comprobar email duplicado si se proporciona
                if (!string.IsNullOrWhiteSpace(Email))
                {
                    var all = await db.GetAllPersonalAsync();
                    var emailExists = all.Exists(p => !string.IsNullOrWhiteSpace(p.Email) && p.Email.Equals(Email.Trim(), StringComparison.OrdinalIgnoreCase));
                    if (emailExists)
                    {
                        await Application.Current.MainPage.DisplayAlert("Validación", "El correo ya está siendo usado por otro usuario.", "OK");
                        return;
                    }
                }

                // Hash simple SHA-256 de la contraseña (mejor usar salt + PBKDF2/Bcrypt en producción)
                string hashedPassword;
                var bytes = Encoding.UTF8.GetBytes(Contrasena);
                var hash = SHA256.HashData(bytes);
                hashedPassword = Convert.ToBase64String(hash);

                // Mapear a tu modelo Personal.
                var personal = new Personal
                {
                    Nombre = NombreCompleto.Trim(),
                    Email = Email?.Trim() ?? string.Empty,
                    Usuario = Usuario.Trim(),
                    Apellido = string.Empty,
                    Contrasena = hashedPassword,
                    Cargo = string.IsNullOrWhiteSpace(Cargo) ? (Departamento ?? string.Empty) : Cargo,
                    Permisos = Permisos ?? string.Empty,
                    FechaContratacion = DateTime.Now
                };

                await db.SavePersonalAsync(personal);

                await Application.Current.MainPage.DisplayAlert("Éxito", "Usuario creado correctamente.", "OK");

                // Volver a la vista principal de Credenciales
                await Shell.Current.GoToAsync(nameof(Pages.Credenciales.CredencialesPrincipal));
            }
            catch (Exception ex)
            {
                // Manejo amistoso de errores: evitar mostrar mensajes crudos de SQLite al usuario
                var message = ex.Message;
                if (message != null && message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase))
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo crear el usuario: ya existe un registro con ese identificador.", "OK");
                else
                    await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo crear el usuario: {ex.Message}", "OK");
            }
        }

        private async Task ExecuteCancelarAsync()
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(Pages.Credenciales.CredencialesPrincipal));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo regresar: {ex.Message}", "OK");
            }
        }
    }
}