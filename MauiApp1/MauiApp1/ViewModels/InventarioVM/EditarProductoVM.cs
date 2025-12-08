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
    internal class EditarProductoVM : ObservableObject
    {
        private readonly DatabaseService? _db;

        public ObservableCollection<Producto> Productos
        {
            get => _productos;
            set => SetProperty(ref _productos, value);
        }
        private ObservableCollection<Producto> _productos = new();

        public Producto? ProductoSeleccionado
        {
            get => _productoSeleccionado;
            set => SetProperty(ref _productoSeleccionado, value);
        }
        private Producto? _productoSeleccionado;

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

        public IAsyncRelayCommand GuardarCommand { get; }
        public IAsyncRelayCommand CancelarCommand { get; }
        public IAsyncRelayCommand CargarCommand { get; }

        public EditarProductoVM()
        {
            var mauiContext = Application.Current?.Handler?.MauiContext;
            _db = mauiContext?.Services.GetService(typeof(DatabaseService)) as DatabaseService;

            GuardarCommand = new AsyncRelayCommand(ExecuteGuardarAsync);
            CancelarCommand = new AsyncRelayCommand(ExecuteCancelarAsync);
            CargarCommand = new AsyncRelayCommand(LoadAsync);
        }

        public async Task LoadAsync()
        {
            if (_db == null) return;

            try
            {
                await _db.InicializarAsync();
                var lista = await _db.GetAllProductosAsync();

                Productos = new ObservableCollection<Producto>(lista);

                // Mantener/actualizar categorías a partir de la BD si hay datos
                var categorias = lista.Select(p => p.Categoria).Where(c => !string.IsNullOrWhiteSpace(c)).Distinct().OrderBy(c => c).ToList();
                if (categorias.Any())
                    CategoriasDisponibles = new ObservableCollection<string>(categorias);

                ProductoSeleccionado = Productos.FirstOrDefault();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo cargar productos: {ex.Message}", "OK");
            }
        }

        private async Task ExecuteGuardarAsync()
        {
            if (_db == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Servicio de base de datos no disponible.", "OK");
                return;
            }

            if (ProductoSeleccionado == null)
            {
                await Application.Current.MainPage.DisplayAlert("Validación", "Seleccione un producto.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(ProductoSeleccionado.Nombre))
            {
                await Application.Current.MainPage.DisplayAlert("Validación", "El nombre del producto no puede estar vacío.", "OK");
                return;
            }

            try
            {
                if (ProductoSeleccionado.PrecioUnitario < 0) ProductoSeleccionado.PrecioUnitario = 0;
                if (ProductoSeleccionado.CostoUnitario < 0) ProductoSeleccionado.CostoUnitario = 0;
                if (ProductoSeleccionado.Existencias < 0) ProductoSeleccionado.Existencias = 0;
                if (ProductoSeleccionado.StockMinimo < 0) ProductoSeleccionado.StockMinimo = 0;

                await _db.SaveProductoAsync(ProductoSeleccionado);

                await Application.Current.MainPage.DisplayAlert("Éxito", "Cambios guardados correctamente.", "OK");

                try
                {
                    await Shell.Current.GoToAsync(nameof(Pages.Inventario.InventarioPrincipal));
                }
                catch
                {
                    if (Shell.Current.Navigation.NavigationStack.Count > 1)
                        await Shell.Current.Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo guardar: {ex.Message}", "OK");
            }
        }

        private async Task ExecuteCancelarAsync()
        {
            if (_db == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Servicio de base de datos no disponible.", "OK");
                return;
            }

            try
            {
                if (ProductoSeleccionado?.IDProducto > 0)
                {
                    var original = await _db.GetProductoByIdAsync(ProductoSeleccionado.IDProducto);
                    if (original != null)
                    {
                        // Restaurar valores desde la BD
                        ProductoSeleccionado.Nombre = original.Nombre;
                        ProductoSeleccionado.Descripcion = original.Descripcion;
                        ProductoSeleccionado.Categoria = original.Categoria;
                        ProductoSeleccionado.PrecioUnitario = original.PrecioUnitario;
                        ProductoSeleccionado.CostoUnitario = original.CostoUnitario;
                        ProductoSeleccionado.StockMinimo = original.StockMinimo;
                        ProductoSeleccionado.Existencias = original.Existencias;

                        // Forzar notificación (si la UI no actualiza campos primitivos)
                        SetProperty(ref _productoSeleccionado, ProductoSeleccionado);
                    }
                }
                else
                {
                    // Si no hay ID, simplemente limpiar
                    ProductoSeleccionado = null;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo cancelar: {ex.Message}", "OK");
            }
        }
    }
}