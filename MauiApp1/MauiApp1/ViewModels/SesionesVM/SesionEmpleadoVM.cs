using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace MauiApp1.ViewModels.SesionesVM
{
    internal class SesionEmpleadoVM : ObservableObject
    {
        public IAsyncRelayCommand GenerarFacturaCommand { get; }
        public IAsyncRelayCommand ComprobarExistenciaCommand { get; }
        public IAsyncRelayCommand RegresarCommand { get; }

        public SesionEmpleadoVM()
        {
            GenerarFacturaCommand = new AsyncRelayCommand(NavigateToFacturaAsync);
            ComprobarExistenciaCommand = new AsyncRelayCommand(NavigateToInventarioAsync);
            RegresarCommand = new AsyncRelayCommand(ExecuteRegresarAsync);
        }

        private async Task NavigateToFacturaAsync()
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(Pages.Empleados.Factura));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo abrir Factura: {ex.Message}", "OK");
            }
        }

        private async Task NavigateToInventarioAsync()
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(Pages.Inventario.InventarioPrincipal));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo abrir Inventario: {ex.Message}", "OK");
            }
        }

        private async Task ExecuteRegresarAsync()
        {
            try
            {
                // Navegar explícitamente a MainPage (cerrar sesión)
                // Uso ruta absoluta para asegurar que volvemos al root/MainPage
                await Shell.Current.GoToAsync("//MainPage");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo cerrar sesión: {ex.Message}", "OK");
            }
        }
    }
}