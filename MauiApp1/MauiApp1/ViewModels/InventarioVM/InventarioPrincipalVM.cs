using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.Modelos;
using MauiApp1.Pages.Inventario;
using MauiApp1.Servicios;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MauiApp1.ViewModels.InventarioVM
{
    internal class InventarioPrincipalVM : ObservableObject
    {
        private readonly DatabaseService? _db;

        public ObservableCollection<Producto> Productos
        {
            get => _productos;
            set => SetProperty(ref _productos, value);
        }
        private ObservableCollection<Producto> _productos = new();

        public ObservableCollection<string> NombresDisponibles
        {
            get => _nombresDisponibles;
            set => SetProperty(ref _nombresDisponibles, value);
        }
        private ObservableCollection<string> _nombresDisponibles = new();

        public ObservableCollection<string> CategoriasDisponibles
        {
            get => _categoriasDisponibles;
            set => SetProperty(ref _categoriasDisponibles, value);
        }
        private ObservableCollection<string> _categoriasDisponibles = new();

        public ObservableCollection<string> RangosPrecio
        {
            get => _rangosPrecio;
            set => SetProperty(ref _rangosPrecio, value);
        }
        private ObservableCollection<string> _rangosPrecio = new();

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

        public string? FiltroCategoria
        {
            get => _filtroCategoria;
            set
            {
                if (SetProperty(ref _filtroCategoria, value))
                    _ = ApplyFilterAsync();
            }
        }
        private string? _filtroCategoria;

        public string? FiltroPrecio
        {
            get => _filtroPrecio;
            set
            {
                if (SetProperty(ref _filtroPrecio, value))
                    _ = ApplyFilterAsync();
            }
        }
        private string? _filtroPrecio;

        public IAsyncRelayCommand AgregarProductoCommand { get; }
        public IAsyncRelayCommand EditarProductoCommand { get; }
        public IAsyncRelayCommand EliminarProductoCommand { get; }
        public IAsyncRelayCommand RegresarAlInicioCommand { get; }
        public IAsyncRelayCommand CargarCommand { get; }

        public InventarioPrincipalVM()
        {
            var mauiContext = Application.Current?.Handler?.MauiContext;
            _db = mauiContext?.Services.GetService(typeof(DatabaseService)) as DatabaseService;

            AgregarProductoCommand = new AsyncRelayCommand(NavigateToAgregarProductoAsync);
            EditarProductoCommand = new AsyncRelayCommand(NavigateToEditarProductoAsync);
            EliminarProductoCommand = new AsyncRelayCommand(NavigateToEliminarProductoAsync);
            // Ahora la navegación depende del rol actual
            RegresarAlInicioCommand = new AsyncRelayCommand(NavigateToSesionPorRolAsync);
            CargarCommand = new AsyncRelayCommand(LoadAsync);

            RangosPrecio = new ObservableCollection<string> { "Todos", "< S/ 50", "S/ 50 - S/ 200", "> S/ 200" };
        }

        public async Task LoadAsync()
        {
            if (_db == null) return;

            try
            {
                await _db.InicializarAsync();
                var lista = await _db.GetAllProductosAsync();

                var nombres = lista.Select(p => p.Nombre).Where(n => !string.IsNullOrWhiteSpace(n)).Distinct().OrderBy(n => n);
                NombresDisponibles = new ObservableCollection<string>(nombres);

                var categorias = lista.Select(p => p.Categoria).Where(c => !string.IsNullOrWhiteSpace(c)).Distinct().OrderBy(c => c).ToList();

                if (categorias.Any())
                {
                    CategoriasDisponibles = new ObservableCollection<string>(categorias);
                }
                else
                {
                    CategoriasDisponibles = new ObservableCollection<string>
                    {
                        "Perfumería",
                        "Bolsos",
                        "Joyería",
                        "Accesorios",
                        "Papelería",
                        "Hogar"
                    };
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    Productos = new ObservableCollection<Producto>(lista);
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo cargar productos: {ex.Message}", "OK");
            }
        }

        private async Task ApplyFilterAsync()
        {
            if (_db == null) return;

            try
            {
                var lista = await _db.GetAllProductosAsync();
                var filtered = lista.AsQueryable();

                if (!string.IsNullOrWhiteSpace(FiltroNombre))
                    filtered = filtered.Where(p => p.Nombre != null && p.Nombre == FiltroNombre);

                if (!string.IsNullOrWhiteSpace(FiltroCategoria))
                    filtered = filtered.Where(p => p.Categoria != null && p.Categoria == FiltroCategoria);

                if (!string.IsNullOrWhiteSpace(FiltroPrecio))
                {
                    switch (FiltroPrecio)
                    {
                        case "< S/ 50":
                            filtered = filtered.Where(p => p.PrecioUnitario < 50);
                            break;
                        case "S/ 50 - S/ 200":
                            filtered = filtered.Where(p => p.PrecioUnitario >= 50 && p.PrecioUnitario <= 200);
                            break;
                        case "> S/ 200":
                            filtered = filtered.Where(p => p.PrecioUnitario > 200);
                            break;
                        default:
                            break;
                    }
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    Productos = new ObservableCollection<Producto>(filtered.ToList());
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error aplicando filtro: {ex.Message}", "OK");
            }
        }

        private async Task NavigateToAgregarProductoAsync()
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(AgregarProducto));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo abrir Agregar Producto: {ex.Message}", "OK");
            }
        }

        private async Task NavigateToEditarProductoAsync()
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(EditarProducto));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo abrir Editar Producto: {ex.Message}", "OK");
            }
        }

        private async Task NavigateToEliminarProductoAsync()
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(EliminarProducto));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo abrir Eliminar Producto: {ex.Message}", "OK");
            }
        }

        private async Task NavigateToSesionPorRolAsync()
        {
            try
            {
                string cargo = Preferences.Get("CurrentUserCargo", string.Empty);

                if (!string.IsNullOrWhiteSpace(cargo) && cargo.Equals("Gerente", StringComparison.OrdinalIgnoreCase))
                {
                    await Shell.Current.GoToAsync(nameof(Pages.Sesiones.SesionGerente));
                }
                else
                {
                    await Shell.Current.GoToAsync(nameof(Pages.Sesiones.SesionEmpleado));
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo regresar al inicio: {ex.Message}", "OK");
            }
        }
    }
}