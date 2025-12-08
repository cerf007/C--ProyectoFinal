using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using MauiApp1.Modelos;
using MauiApp1.Servicios;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MauiApp1.ViewModels.CredencialesVM
{
    internal class EliminarEmpleadoVM : ObservableObject
    {
        private readonly DatabaseService? _db;

        public ObservableCollection<Personal> EmpleadosFiltrados
        {
            get => _empleadosFiltrados;
            set => SetProperty(ref _empleadosFiltrados, value);
        }
        private ObservableCollection<Personal> _empleadosFiltrados = new();

        public Personal? EmpleadoSeleccionado
        {
            get => _empleadoSeleccionado;
            set => SetProperty(ref _empleadoSeleccionado, value);
        }
        private Personal? _empleadoSeleccionado;

        public ObservableCollection<string> CargosDisponibles { get => _cargosDisponibles; set => SetProperty(ref _cargosDisponibles, value); }
        private ObservableCollection<string> _cargosDisponibles = new();

        public ObservableCollection<string> PermisosDisponibles { get => _permisosDisponibles; set => SetProperty(ref _permisosDisponibles, value); }
        private ObservableCollection<string> _permisosDisponibles = new();

        public string? FiltroCargo
        {
            get => _filtroCargo;
            set => SetProperty(ref _filtroCargo, value);
        }
        private string? _filtroCargo;

        public string? FiltroPermisos
        {
            get => _filtroPermisos;
            set => SetProperty(ref _filtroPermisos, value);
        }
        private string? _filtroPermisos;

        public string? FiltroNombre
        {
            get => _filtroNombre;
            set => SetProperty(ref _filtroNombre, value);
        }
        private string? _filtroNombre;

        // Controla la visibilidad del panel de confirmación
        public bool MostrarConfirmacion
        {
            get => _mostrarConfirmacion;
            set => SetProperty(ref _mostrarConfirmacion, value);
        }
        private bool _mostrarConfirmacion = false;

        // Comandos
        public IAsyncRelayCommand CargarCommand { get; }
        public IAsyncRelayCommand AplicarFiltroCommand { get; }
        public IAsyncRelayCommand PrepararEliminarCommand { get; }
        public IAsyncRelayCommand ConfirmarEliminarCommand { get; }
        public IAsyncRelayCommand CancelarConfirmacionCommand { get; }
        public IAsyncRelayCommand ComandoRegresar { get; }

        public EliminarEmpleadoVM()
        {
            var mauiContext = Application.Current?.Handler?.MauiContext;
            _db = mauiContext?.Services.GetService(typeof(DatabaseService)) as DatabaseService;

            CargarCommand = new AsyncRelayCommand(LoadAsync);
            AplicarFiltroCommand = new AsyncRelayCommand(ApplyFilterAsync);
            PrepararEliminarCommand = new AsyncRelayCommand(ExecutePrepararEliminarAsync);
            ConfirmarEliminarCommand = new AsyncRelayCommand(ExecuteConfirmarEliminarAsync);
            CancelarConfirmacionCommand = new AsyncRelayCommand(ExecuteCancelarConfirmacionAsync);
            ComandoRegresar = new AsyncRelayCommand(async () => await Shell.Current.GoToAsync(nameof(Pages.Credenciales.CredencialesPrincipal)));
        }

        public async Task LoadAsync()
        {
            if (_db == null) return;

            try
            {
                await _db.InicializarAsync();
                var lista = await _db.GetAllPersonalAsync();

                var cargos = lista.Select(p => p.Cargo).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().OrderBy(s => s);
                CargosDisponibles = new ObservableCollection<string>(cargos);

                var permisos = lista.SelectMany(p => (p.Permisos ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                                    .Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().OrderBy(s => s);
                PermisosDisponibles = new ObservableCollection<string>(permisos);

                Device.BeginInvokeOnMainThread(() =>
                {
                    EmpleadosFiltrados = new ObservableCollection<Personal>(lista);
                    MostrarConfirmacion = false;
                    EmpleadoSeleccionado = null;
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo cargar empleados: {ex.Message}", "OK");
            }
        }

        private async Task ApplyFilterAsync()
        {
            if (_db == null) return;

            try
            {
                var lista = await _db.GetAllPersonalAsync();
                var filtered = lista.AsQueryable();

                if (!string.IsNullOrWhiteSpace(FiltroCargo))
                    filtered = filtered.Where(p => p.Cargo == FiltroCargo);

                if (!string.IsNullOrWhiteSpace(FiltroPermisos))
                    filtered = filtered.Where(p => (p.Permisos ?? string.Empty).Contains(FiltroPermisos, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrWhiteSpace(FiltroNombre))
                    filtered = filtered.Where(p => (p.Nombre ?? string.Empty).Contains(FiltroNombre, StringComparison.OrdinalIgnoreCase));

                Device.BeginInvokeOnMainThread(() =>
                {
                    EmpleadosFiltrados = new ObservableCollection<Personal>(filtered.ToList());
                    EmpleadoSeleccionado = null;
                    MostrarConfirmacion = false;
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error aplicando filtro: {ex.Message}", "OK");
            }
        }

        private async Task ExecutePrepararEliminarAsync()
        {
            if (EmpleadoSeleccionado == null)
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "Selecciona primero un empleado.", "OK");
                return;
            }

            // Mostrar panel de confirmación (no borrar todavía)
            MostrarConfirmacion = true;
        }

        private async Task ExecuteConfirmarEliminarAsync()
        {
            if (EmpleadoSeleccionado == null)
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "No hay empleado seleccionado.", "OK");
                return;
            }

            try
            {
                // Confirmación final modal
                var confirmar = await Application.Current.MainPage.DisplayAlert(
                    "Confirmar eliminación",
                    $"¿Eliminar permanentemente a {EmpleadoSeleccionado.Nombre}?",
                    "Sí, eliminar", "Cancelar");

                if (!confirmar)
                {
                    MostrarConfirmacion = false;
                    return;
                }

                if (_db == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Servicio de base de datos no disponible.", "OK");
                    return;
                }

                await _db.DeletePersonalAsync(EmpleadoSeleccionado);
                await Application.Current.MainPage.DisplayAlert("Éxito", "Empleado eliminado.", "OK");

                // Recargar lista y ocultar panel
                await LoadAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo eliminar: {ex.Message}", "OK");
            }
        }

        private Task ExecuteCancelarConfirmacionAsync()
        {
            MostrarConfirmacion = false;
            return Task.CompletedTask;
        }
    }
}