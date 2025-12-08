using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using MauiApp1.Pages.Credenciales;
using MauiApp1.Pages.Inventario;
using MauiApp1.Pages.Proveedores;
using MauiApp1.Pages.Reportes;

namespace MauiApp1.ViewModels.SesionesVM
{
    internal class SesionGerenteVM : ObservableObject
    {
        public IAsyncRelayCommand IngresarCredencialesCommand { get; }
        public IAsyncRelayCommand IngresarInventarioCommand { get; }
        public IAsyncRelayCommand IngresarProveedoresCommand { get; }
        public IAsyncRelayCommand IngresarReportesCommand { get; }
        public IAsyncRelayCommand CerrarSesionCommand { get; }

        public SesionGerenteVM()
        {
            IngresarCredencialesCommand = new AsyncRelayCommand(NavigateToCredencialesAsync);
            IngresarInventarioCommand = new AsyncRelayCommand(NavigateToInventarioAsync);
            IngresarProveedoresCommand = new AsyncRelayCommand(NavigateToProveedoresAsync);
            IngresarReportesCommand = new AsyncRelayCommand(NavigateToReportesAsync);
            CerrarSesionCommand = new AsyncRelayCommand(ExecuteCerrarSesionAsync);
        }

        private async Task NavigateToCredencialesAsync()
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(CredencialesPrincipal));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo abrir Credenciales: {ex.Message}", "OK");
            }
        }

        private async Task NavigateToInventarioAsync()
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(MauiApp1.Pages.Inventario.InventarioPrincipal));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo abrir Inventario: {ex.Message}", "OK");
            }
        }

        private async Task NavigateToProveedoresAsync()
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(ProveedoresPrincipal));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo abrir Proveedores: {ex.Message}", "OK");
            }
        }

        private async Task NavigateToReportesAsync()
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(ReportesPrincipal));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo abrir Reportes: {ex.Message}", "OK");
            }
        }

        private async Task ExecuteCerrarSesionAsync()
        {
            try
            {
                // Ajusta la navegación según tu flujo de login
                await Shell.Current.GoToAsync("//MainPage");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo cerrar sesión: {ex.Message}", "OK");
            }
        }
    }
}