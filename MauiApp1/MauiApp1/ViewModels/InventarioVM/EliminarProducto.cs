using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.Modelos;
using MauiApp1.Servicios;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MauiApp1.ViewModels.InventarioVM
{
    internal class EliminarProductoVM : ObservableObject
    {
        private readonly DatabaseService? _db;

        public ObservableCollection<Producto> Productos
        {
            get => _productos;
            set => SetProperty(ref _productos, value);
        }
        private ObservableCollection<Producto> _productos = new();

        // Lista que se muestra en el Picker (filtrada)
        public ObservableCollection<Producto> ProductosFiltrados
        {
            get => _productosFiltrados;
            set => SetProperty(ref _productosFiltrados, value);
        }
        private ObservableCollection<Producto> _productosFiltrados = new();

        public ObservableCollection<string> CategoriasDisponibles
        {
            get => _categoriasDisponibles;
            set => SetProperty(ref _categoriasDisponibles, value);
        }
        private ObservableCollection<string> _categoriasDisponibles = new ObservableCollection<string>
        {
            "Perfumería",
            "Bolsos",
            "Joyería",
            "Accesorios",
            "Papelería",
            "Hogar"
        };

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

        public Producto? ProductoSeleccionado
        {
            get => _productoSeleccionado;
            set
            {
                if (SetProperty(ref _productoSeleccionado, value))
                {
                    // Actualizar visibilidad del panel de confirmación
                    MostrarConfirmacion = _productoSeleccionado != null;
                }
            }
        }
        private Producto? _productoSeleccionado;

        public bool MostrarConfirmacion
        {
            get => _mostrarConfirmacion;
            set => SetProperty(ref _mostrarConfirmacion, value);
        }
        private bool _mostrarConfirmacion;

        public IAsyncRelayCommand CargarCommand { get; }
        public IAsyncRelayCommand AplicarFiltroCommand { get; }
        public IAsyncRelayCommand EliminarCommand { get; }
        public IAsyncRelayCommand CancelarCommand { get; }

        public EliminarProductoVM()
        {
            var mauiContext = Application.Current?.Handler?.MauiContext;
            _db = mauiContext?.Services.GetService(typeof(DatabaseService)) as DatabaseService;

            CargarCommand = new AsyncRelayCommand(LoadAsync);
            AplicarFiltroCommand = new AsyncRelayCommand(ApplyFilterAsync);
            EliminarCommand = new AsyncRelayCommand(ExecuteEliminarAsync);
            CancelarCommand = new AsyncRelayCommand(ExecuteCancelarAsync);
        }

        public async Task LoadAsync()
        {
            if (_db == null) return;

            try
            {
                await _db.InicializarAsync();
                var lista = await _db.GetAllProductosAsync();

                Productos = new ObservableCollection<Producto>(lista);
                ProductosFiltrados = new ObservableCollection<Producto>(lista);

                var categorias = lista.Select(p => p.Categoria)
                                      .Where(c => !string.IsNullOrWhiteSpace(c))
                                      .Distinct()
                                      .OrderBy(c => c)
                                      .ToList();

                if (categorias.Any())
                    CategoriasDisponibles = new ObservableCollection<string>(categorias);

                // Reset filtros
                FiltroNombre = string.Empty;
                FiltroCategoria = null;
                ProductoSeleccionado = null;
                MostrarConfirmacion = false;
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
                var lista = Productos.ToList().AsQueryable();

                if (!string.IsNullOrWhiteSpace(FiltroCategoria))
                    lista = lista.Where(p => p.Categoria != null && p.Categoria == FiltroCategoria);

                if (!string.IsNullOrWhiteSpace(FiltroNombre))
                    lista = lista.Where(p => p.Nombre != null && p.Nombre.IndexOf(FiltroNombre, StringComparison.OrdinalIgnoreCase) >= 0);

                var result = lista.ToList();

                ProductosFiltrados = new ObservableCollection<Producto>(result);

                // Si el seleccionado ya no está en la lista, deseleccionar
                if (ProductoSeleccionado != null && !ProductosFiltrados.Any(p => p.IDProducto == ProductoSeleccionado.IDProducto))
                {
                    ProductoSeleccionado = null;
                    MostrarConfirmacion = false;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error aplicando filtro: {ex.Message}", "OK");
            }
        }

        private async Task ExecuteEliminarAsync()
        {
            if (_db == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Servicio de base de datos no disponible.", "OK");
                return;
            }

            if (ProductoSeleccionado == null)
            {
                await Application.Current.MainPage.DisplayAlert("Validación", "Seleccione un producto para eliminar.", "OK");
                return;
            }

            var confirmed = await Application.Current.MainPage.DisplayAlert(
                "Confirmar eliminación",
                $"¿Eliminar '{ProductoSeleccionado.Nombre}'? Esta acción no se puede deshacer.",
                "Eliminar",
                "Cancelar");

            if (!confirmed) return;

            try
            {
                await _db.DeleteProductoAsync(ProductoSeleccionado);

                await Application.Current.MainPage.DisplayAlert("Éxito", "Producto eliminado correctamente.", "OK");

                // Refrescar lista y resetear selección
                await LoadAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo eliminar: {ex.Message}", "OK");
            }
        }

        private Task ExecuteCancelarAsync()
        {
            // Ocultar el panel de confirmación y deseleccionar
            ProductoSeleccionado = null;
            MostrarConfirmacion = false;
            return Task.CompletedTask;
        }
    }
}