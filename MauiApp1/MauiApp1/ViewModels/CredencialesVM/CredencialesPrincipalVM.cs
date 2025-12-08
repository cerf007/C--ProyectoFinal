using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using MauiApp1.Pages.Credenciales;
using System.Collections.ObjectModel;
using MauiApp1.Modelos;
using MauiApp1.Servicios;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace MauiApp1.ViewModels.CredencialesVM
{
    internal class CredencialesPrincipalVM : ObservableObject
    {
        private readonly DatabaseService? _db;

        public ObservableCollection<Personal> PersonalFiltrado
        {
            get => _personalFiltrado;
            set => SetProperty(ref _personalFiltrado, value);
        }
        private ObservableCollection<Personal> _personalFiltrado = new();

        public ObservableCollection<string> NombresDisponibles { get => _nombresDisponibles; set => SetProperty(ref _nombresDisponibles, value); }
        private ObservableCollection<string> _nombresDisponibles = new();

        public ObservableCollection<string> PermisosDisponibles { get => _permisosDisponibles; set => SetProperty(ref _permisosDisponibles, value); }
        private ObservableCollection<string> _permisosDisponibles = new();

        public ObservableCollection<string> CargosDisponibles { get => _cargosDisponibles; set => SetProperty(ref _cargosDisponibles, value); }
        private ObservableCollection<string> _cargos_disponibles_warning; // not used directly
        private ObservableCollection<string> _cargosDisponibles = new();

        public string? FiltroNombre
        {
            get => _filtroNombre;
            set
            {
                if (SetProperty(ref _filtroNombre, value))
                    _ = ApplyFilterAsync();
            }
        }
        private string? _filtroNombre;

        public string? FiltroPermisos
        {
            get => _filtroPermisos;
            set
            {
                if (SetProperty(ref _filtroPermisos, value))
                    _ = ApplyFilterAsync();
            }
        }
        private string? _filtroPermisos;

        public string? FiltroCargo
        {
            get => _filtroCargo;
            set
            {
                if (SetProperty(ref _filtroCargo, value))
                    _ = ApplyFilterAsync();
            }
        }
        private string? _filtroCargo;

        public IAsyncRelayCommand AgregarUsuarioCommand { get; }
        public IAsyncRelayCommand EditarUsuarioCommand { get; }
        public IAsyncRelayCommand EliminarUsuarioCommand { get; }
        public IAsyncRelayCommand CargarCommand { get; }

        // Nuevo comando para restablecer contraseña; recibe la entidad Personal como parámetro
        public IAsyncRelayCommand<Personal> RestablecerContrasenaCommand { get; }

        // Comando Regresar (volver a SesionGerente)
        public IAsyncRelayCommand RegresarCommand { get; }

        public CredencialesPrincipalVM()
        {
            var mauiContext = Application.Current?.Handler?.MauiContext;
            _db = mauiContext?.Services.GetService<DatabaseService>();

            AgregarUsuarioCommand = new AsyncRelayCommand(NavigateToAgregarEmpleadoAsync);
            EditarUsuarioCommand = new AsyncRelayCommand(NavigateToEditarEmpleadoAsync);
            EliminarUsuarioCommand = new AsyncRelayCommand(NavigateToEliminarEmpleadoAsync);
            CargarCommand = new AsyncRelayCommand(LoadAsync);

            RestablecerContrasenaCommand = new AsyncRelayCommand<Personal>(ExecuteRestablecerContrasenaAsync);

            RegresarCommand = new AsyncRelayCommand(NavigateToSesionGerenteAsync);
        }

        public async Task LoadAsync()
        {
            if (_db == null) return;

            try
            {
                await _db.InicializarAsync();
                var lista = await _db.GetAllPersonalAsync();

                // poblar pickers
                var nombres = lista.Select(p => p.Nombre).Where(n => !string.IsNullOrWhiteSpace(n)).Distinct().OrderBy(n => n);
                NombresDisponibles = new ObservableCollection<string>(nombres);

                var permisos = lista.SelectMany(p => (p.Permisos ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                                    .Where(s => !string.IsNullOrWhiteSpace(s))
                                    .Distinct()
                                    .OrderBy(s => s);
                PermisosDisponibles = new ObservableCollection<string>(permisos);

                var cargos = lista.Select(p => p.Cargo).Where(c => !string.IsNullOrWhiteSpace(c)).Distinct().OrderBy(c => c);
                CargosDisponibles = new ObservableCollection<string>(cargos);

                Device.BeginInvokeOnMainThread(() =>
                {
                    PersonalFiltrado = new ObservableCollection<Personal>(lista);
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo cargar personal: {ex.Message}", "OK");
            }
        }

        private async Task ApplyFilterAsync()
        {
            if (_db == null) return;

            try
            {
                var lista = await _db.GetAllPersonalAsync();
                var filtered = lista.AsQueryable();

                if (!string.IsNullOrWhiteSpace(FiltroNombre))
                    filtered = filtered.Where(p => p.Nombre == FiltroNombre);

                if (!string.IsNullOrWhiteSpace(FiltroPermisos))
                    filtered = filtered.Where(p => (p.Permisos ?? string.Empty).Contains(FiltroPermisos, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrWhiteSpace(FiltroCargo))
                    filtered = filtered.Where(p => p.Cargo == FiltroCargo);

                Device.BeginInvokeOnMainThread(() =>
                {
                    PersonalFiltrado = new ObservableCollection<Personal>(filtered.ToList());
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error aplicando filtro: {ex.Message}", "OK");
            }
        }

        private async Task NavigateToAgregarEmpleadoAsync()
        {
            await Shell.Current.GoToAsync(nameof(AgregarEmpleado));
        }

        private async Task NavigateToEditarEmpleadoAsync()
        {
            await Shell.Current.GoToAsync(nameof(EditarEmpleado));
        }

        private async Task NavigateToEliminarEmpleadoAsync()
        {
            await Shell.Current.GoToAsync(nameof(EliminarEmpleado));
        }

        private async Task ExecuteRestablecerContrasenaAsync(Personal personal)
        {
            if (personal == null)
                return;

            try
            {
                var nueva = await Application.Current.MainPage.DisplayPromptAsync(
                    "Restablecer contraseña",
                    $"Introduce nueva contraseña para {personal.Nombre} (vacío = cancelar):",
                    "Aceptar", "Cancelar", "Nueva contraseña", -1, Keyboard.Text, string.Empty);

                if (string.IsNullOrWhiteSpace(nueva))
                {
                    return;
                }

                var bytes = Encoding.UTF8.GetBytes(nueva);
                var hash = SHA256.HashData(bytes);
                personal.Contrasena = Convert.ToBase64String(hash);

                if (_db == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Servicio de BD no disponible.", "OK");
                    return;
                }

                await _db.SavePersonalAsync(personal);
                await Application.Current.MainPage.DisplayAlert("Éxito", $"Contraseña actualizada para {personal.Nombre}.", "OK");

                await LoadAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo restablecer: {ex.Message}", "OK");
            }
        }

        private async Task NavigateToSesionGerenteAsync()
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(Pages.Sesiones.SesionGerente));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo regresar: {ex.Message}", "OK");
            }
        }
    }
}