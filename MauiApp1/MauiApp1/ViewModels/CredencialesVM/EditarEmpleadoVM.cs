using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using MauiApp1.Modelos;
using MauiApp1.Servicios;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace MauiApp1.ViewModels.CredencialesVM
{
    internal class EditarEmpleadoVM : ObservableObject
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
            set
            {
                if (SetProperty(ref _empleadoSeleccionado, value))
                {
                    // Resetear campo de edición de contraseña al cambiar selección
                    ContrasenaEdicion = string.Empty;
                }
            }
        }
        private Personal? _empleadoSeleccionado;

        // Campo de edición de contraseña (texto plano). No se muestra el hash.
        public string ContrasenaEdicion
        {
            get => _contrasenaEdicion;
            set => SetProperty(ref _contrasenaEdicion, value);
        }
        private string _contrasenaEdicion = string.Empty;

        // Nuevas colecciones para los Pickers
        public ObservableCollection<string> CargosDisponibles
        {
            get => _cargosDisponibles;
            set => SetProperty(ref _cargosDisponibles, value);
        }
        private ObservableCollection<string> _cargosDisponibles = new();

        public ObservableCollection<string> PermisosDisponibles
        {
            get => _permisosDisponibles;
            set => SetProperty(ref _permisosDisponibles, value);
        }
        private ObservableCollection<string> _permisosDisponibles = new();

        // Filtros enlazables desde la vista
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

        public IAsyncRelayCommand GuardarCambiosCommand { get; }
        public IAsyncRelayCommand CancelarCommand { get; }
        public IAsyncRelayCommand VolverCommand { get; }
        public IAsyncRelayCommand CargarCommand { get; }

        public EditarEmpleadoVM()
        {
            // Resolver servicio de BD desde MauiContext (compatible con MAUI DI en runtime)
            var mauiContext = Application.Current?.Handler?.MauiContext;
            _db = mauiContext?.Services.GetService(typeof(DatabaseService)) as DatabaseService;

            GuardarCambiosCommand = new AsyncRelayCommand(ExecuteGuardarCambiosAsync);
            CancelarCommand = new AsyncRelayCommand(ExecuteCancelarAsync);
            VolverCommand = new AsyncRelayCommand(ExecuteVolverAsync);
            CargarCommand = new AsyncRelayCommand(LoadAsync);

            // Valores por defecto para Pickers (puedes poblar dinámicamente desde BD)
            if (!CargosDisponibles.Any())
            {
                CargosDisponibles = new ObservableCollection<string>
                {
                    "Vendedor",
                    "Administrador",
                    "Operario"
                };
            }

            if (!PermisosDisponibles.Any())
            {
                PermisosDisponibles = new ObservableCollection<string>
                {
                    "Generar Facturas",
                    "Ver Inventario",
                    "Administrar Usuarios"
                };
            }
        }

        private bool EmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        public async Task LoadAsync()
        {
            if (_db == null) return;

            try
            {
                await _db.InicializarAsync();
                var lista = await _db.GetAllPersonalAsync();

                var cargosFromDb = lista.Select(p => p.Cargo).Where(c => !string.IsNullOrWhiteSpace(c)).Distinct().ToList();
                foreach (var c in cargosFromDb)
                    if (!CargosDisponibles.Contains(c))
                        CargosDisponibles.Add(c);

                var permisosFromDb = lista.SelectMany(p => (p.Permisos ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                                         .Where(s => !string.IsNullOrWhiteSpace(s))
                                         .Distinct().ToList();
                foreach (var p in permisosFromDb)
                    if (!PermisosDisponibles.Contains(p))
                        PermisosDisponibles.Add(p);

                Device.BeginInvokeOnMainThread(() =>
                {
                    EmpleadosFiltrados = new ObservableCollection<Personal>(lista);
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

                Device.BeginInvokeOnMainThread(() =>
                {
                    EmpleadosFiltrados = new ObservableCollection<Personal>(filtered.ToList());
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error aplicando filtro: {ex.Message}", "OK");
            }
        }

        private async Task ExecuteGuardarCambiosAsync()
        {
            if (EmpleadoSeleccionado == null)
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "Selecciona primero un empleado.", "OK");
                return;
            }

            // Validaciones mínimas
            if (string.IsNullOrWhiteSpace(EmpleadoSeleccionado.Nombre) || EmpleadoSeleccionado.Nombre.Trim().Length < 3)
            {
                await Application.Current.MainPage.DisplayAlert("Validación", "Nombre inválido (mín. 3 caracteres).", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(EmpleadoSeleccionado.Usuario) || EmpleadoSeleccionado.Usuario.Trim().Length < 3)
            {
                await Application.Current.MainPage.DisplayAlert("Validación", "Usuario inválido (mín. 3 caracteres).", "OK");
                return;
            }

            if (!string.IsNullOrWhiteSpace(EmpleadoSeleccionado.Email) && !EmailValido(EmpleadoSeleccionado.Email))
            {
                await Application.Current.MainPage.DisplayAlert("Validación", "Correo electrónico inválido.", "OK");
                return;
            }

            try
            {
                // Si se rellenó la contraseña, calcular hash y asignar
                if (!string.IsNullOrWhiteSpace(ContrasenaEdicion))
                {
                    if (ContrasenaEdicion.Length < 6)
                    {
                        await Application.Current.MainPage.DisplayAlert("Validación", "La nueva contraseña debe tener al menos 6 caracteres.", "OK");
                        return;
                    }

                    var bytes = Encoding.UTF8.GetBytes(ContrasenaEdicion);
                    var hash = SHA256.HashData(bytes);
                    EmpleadoSeleccionado.Contrasena = Convert.ToBase64String(hash);
                    // limpiar campo plano para seguridad
                    ContrasenaEdicion = string.Empty;
                }

                if (_db == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Servicio de base de datos no disponible.", "OK");
                    return;
                }

                // Comprobar que el usuario no exista bajo otro ID (unicidad)
                var existente = await _db.GetPersonalPorUsuarioAsync(EmpleadoSeleccionado.Usuario.Trim());
                if (existente != null && existente.IDPersonal != EmpleadoSeleccionado.IDPersonal)
                {
                    await Application.Current.MainPage.DisplayAlert("Validación", "El nombre de usuario ya está en uso por otro empleado.", "OK");
                    return;
                }

                // (Opcional) comprobar email duplicado en otro registro
                if (!string.IsNullOrWhiteSpace(EmpleadoSeleccionado.Email))
                {
                    var all = await _db.GetAllPersonalAsync();
                    var emailConflict = all.FirstOrDefault(p => p.IDPersonal != EmpleadoSeleccionado.IDPersonal
                                                                && !string.IsNullOrWhiteSpace(p.Email)
                                                                && p.Email.Equals(EmpleadoSeleccionado.Email.Trim(), StringComparison.OrdinalIgnoreCase));
                    if (emailConflict != null)
                    {
                        await Application.Current.MainPage.DisplayAlert("Validación", "El correo ya está siendo usado por otro usuario.", "OK");
                        return;
                    }
                }

                await _db.SavePersonalAsync(EmpleadoSeleccionado);

                await Application.Current.MainPage.DisplayAlert("Éxito", "Cambios guardados.", "OK");

                // Refrescar lista
                await LoadAsync();
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                if (message != null && message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase))
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo guardar: conflicto de unicidad.", "OK");
                else
                    await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo guardar: {ex.Message}", "OK");
            }
        }

        private async Task ExecuteCancelarAsync()
        {
            // Simplemente limpiar la selección o recargar
            EmpleadoSeleccionado = null;
            ContrasenaEdicion = string.Empty;
            await Task.CompletedTask;
        }

        private async Task ExecuteVolverAsync()
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