using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using MauiApp1.Modelos;
using MauiApp1.Servicios;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MauiApp1.ViewModels.ProveedoresVM
{
    internal class ProveedoresPrincipalVM : ObservableObject
    {
        private readonly DatabaseService? _db;

        public ObservableCollection<Proveedor> Proveedores
        {
            get => _proveedores;
            set => SetProperty(ref _proveedores, value);
        }
        private ObservableCollection<Proveedor> _proveedores = new();

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    _ = ApplyFilterAsync();
            }
        }
        private string _searchText = string.Empty;

        public IAsyncRelayCommand AgregarProveedorCommand { get; }
        public IAsyncRelayCommand RemoverProveedorCommand { get; }
        public IAsyncRelayCommand RegresarInicio { get; }

        public IAsyncRelayCommand CargarCommand { get; }
        public IAsyncRelayCommand BuscarProveedorCommand { get; }

        public ProveedoresPrincipalVM()
        {
            var mauiContext = Application.Current?.Handler?.MauiContext;
            _db = mauiContext?.Services.GetService(typeof(DatabaseService)) as DatabaseService;

            AgregarProveedorCommand = new AsyncRelayCommand(NavigateToAgregarProveedorAsync);
            RemoverProveedorCommand = new AsyncRelayCommand(NavigateToEliminarProveedorAsync);
            RegresarInicio = new AsyncRelayCommand(ExecuteRegresarInicioAsync);

            CargarCommand = new AsyncRelayCommand(LoadAsync);
            BuscarProveedorCommand = new AsyncRelayCommand(ApplyFilterAsync);
        }

        public async Task LoadAsync()
        {
            if (_db == null) return;

            try
            {
                await _db.InicializarAsync();
                var lista = await _db.GetAllProveedoresAsync();

                Device.BeginInvokeOnMainThread(() =>
                {
                    Proveedores = new ObservableCollection<Proveedor>(lista);
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo cargar proveedores: {ex.Message}", "OK");
            }
        }

        private async Task ApplyFilterAsync()
        {
            if (_db == null) return;

            try
            {
                var lista = await _db.GetAllProveedoresAsync();

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    var q = SearchText.Trim();
                    lista = lista.Where(p =>
                        (!string.IsNullOrEmpty(p.Nombre) && p.Nombre.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(p.Categoria) && p.Categoria.Contains(q, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    Proveedores = new ObservableCollection<Proveedor>(lista);
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error aplicando filtro: {ex.Message}", "OK");
            }
        }

        private async Task NavigateToAgregarProveedorAsync()
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(Pages.Proveedores.AgregarProveedores));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo abrir Agregar Proveedor: {ex.Message}", "OK");
            }
        }

        private async Task NavigateToEliminarProveedorAsync()
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(Pages.Proveedores.EliminarProveedores));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo abrir Eliminar Proveedor: {ex.Message}", "OK");
            }
        }

        private async Task ExecuteRegresarInicioAsync()
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