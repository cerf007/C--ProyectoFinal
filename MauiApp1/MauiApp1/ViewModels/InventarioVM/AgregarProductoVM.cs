using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using MauiApp1.Modelos;
using MauiApp1.Servicios;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MauiApp1.ViewModels.InventarioVM
{
    internal class AgregarProductoVM : ObservableObject
    {
        private readonly DatabaseService? _db;

        public Producto NuevoProducto
        {
            get => _nuevoProducto;
            set => SetProperty(ref _nuevoProducto, value);
        }
        private Producto _nuevoProducto = new Producto();

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
        public IAsyncRelayCommand ComandoRegresar { get; }

        public AgregarProductoVM()
        {
            var mauiContext = Application.Current?.Handler?.MauiContext;
            _db = mauiContext?.Services.GetService(typeof(DatabaseService)) as DatabaseService;

            GuardarCommand = new AsyncRelayCommand(ExecuteGuardarAsync);
            ComandoRegresar = new AsyncRelayCommand(ExecuteRegresarAsync);

            // Inicializar valores por defecto seguros
            NuevoProducto = new Producto
            {
                Nombre = string.Empty,
                Descripcion = string.Empty,
                PrecioUnitario = 0,
                CostoUnitario = 0,
                Existencias = 0,
                StockMinimo = 0,
                Categoria = CategoriasDisponibles.Count > 0 ? CategoriasDisponibles[0] : string.Empty
            };
        }

        private bool NombreValido(string nombre)
        {
            return !string.IsNullOrWhiteSpace(nombre) && nombre.Trim().Length >= 2;
        }

        private async Task<bool> ConfirmarSiPrecioMenorQueCostoAsync()
        {
            if (NuevoProducto.PrecioUnitario < NuevoProducto.CostoUnitario)
            {
                return await Application.Current.MainPage.DisplayAlert(
                    "Validación",
                    "El precio unitario es menor que el costo unitario. ¿Desea continuar de todas formas?",
                    "Continuar",
                    "Cancelar");
            }
            return true;
        }

        private async Task ExecuteGuardarAsync()
        {
            // Validaciones básicas
            if (!NombreValido(NuevoProducto?.Nombre))
            {
                await Application.Current.MainPage.DisplayAlert("Validación", "Ingrese el nombre del producto (mín. 2 caracteres).", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(NuevoProducto.Categoria))
            {
                await Application.Current.MainPage.DisplayAlert("Validación", "Seleccione una categoría.", "OK");
                return;
            }

            // Valores numéricos no negativos
            if (NuevoProducto.PrecioUnitario < 0) NuevoProducto.PrecioUnitario = 0;
            if (NuevoProducto.CostoUnitario < 0) NuevoProducto.CostoUnitario = 0;
            if (NuevoProducto.Existencias < 0) NuevoProducto.Existencias = 0;
            if (NuevoProducto.StockMinimo < 0) NuevoProducto.StockMinimo = 0;

            try
            {
                if (_db == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Servicio de base de datos no disponible.", "OK");
                    return;
                }

                // Comprobar duplicados por nombre (case-insensitive)
                var lista = await _db.GetAllProductosAsync();
                var dup = lista.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.Nombre)
                                                    && p.Nombre.Trim().Equals(NuevoProducto.Nombre?.Trim(), StringComparison.OrdinalIgnoreCase));
                if (dup != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Validación", "Ya existe un producto con ese nombre.", "OK");
                    return;
                }

                // Verificar inconsistencia lógica: precio < costo
                var continuar = await ConfirmarSiPrecioMenorQueCostoAsync();
                if (!continuar)
                    return;

                await _db.SaveProductoAsync(NuevoProducto);

                await Application.Current.MainPage.DisplayAlert("Éxito", "Producto guardado correctamente.", "OK");

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
                var msg = ex.Message ?? "Error interno";
                if (msg.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase))
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo guardar: conflicto de unicidad.", "OK");
                else
                    await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo guardar el producto: {msg}", "OK");
            }
        }

        private async Task ExecuteRegresarAsync()
        {
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
    }
}