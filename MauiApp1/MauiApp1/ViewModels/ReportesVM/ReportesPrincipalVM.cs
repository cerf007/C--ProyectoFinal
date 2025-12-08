using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MauiApp1.ViewModels.ReportesVM
{
    internal class ReportesPrincipalVM : ObservableObject
    {
        public ObservableCollection<string> Reportes
        {
            get => _reportes;
            set => SetProperty(ref _reportes, value);
        }
        private ObservableCollection<string> _reportes = new();

        public string? ReporteSeleccionado
        {
            get => _reporteSeleccionado;
            set => SetProperty(ref _reporteSeleccionado, value);
        }
        private string? _reporteSeleccionado;

        public IAsyncRelayCommand CargarCommand { get; }
        public IAsyncRelayCommand<string> VerDetalleCommand { get; }
        public IAsyncRelayCommand ExportarCommand { get; }
        public IAsyncRelayCommand RegresarCommand { get; }

        public ReportesPrincipalVM()
        {
            CargarCommand = new AsyncRelayCommand(LoadAsync);
            VerDetalleCommand = new AsyncRelayCommand<string>(ExecuteVerDetalleAsync);
            ExportarCommand = new AsyncRelayCommand(ExecuteExportarAsync);
            RegresarCommand = new AsyncRelayCommand(ExecuteRegresarAsync);

            // Carga inicial
            _ = CargarCommand.ExecuteAsync(null);
        }

        private Task LoadAsync()
        {
            // Datos de ejemplo — puedes cargarlos desde BD o servicio
            Reportes = new ObservableCollection<string>
            {
                "Reporte diario",
                "Reporte mensual",
                "Reporte trimestral",
                "Reporte anual"
            };

            ReporteSeleccionado = null;
            return Task.CompletedTask;
        }

        private async Task ExecuteVerDetalleAsync(string nombreReporte)
        {
            if (string.IsNullOrWhiteSpace(nombreReporte))
            {
                await Application.Current.MainPage.DisplayAlert("Info", "Seleccione un reporte.", "OK");
                return;
            }

            await Application.Current.MainPage.DisplayAlert("Detalle de reporte", $"Mostrando detalles para: {nombreReporte}", "OK");
        }

        private async Task ExecuteExportarAsync()
        {
            // Placeholder: reemplaza con lógica real (CSV, PDF, etc.)
            await Application.Current.MainPage.DisplayAlert("Exportar", "Funcionalidad de exportación no implementada aún.", "OK");
        }

        private async Task ExecuteRegresarAsync()
        {
            try
            {
                // Volver a la sesión del gerente
                await Shell.Current.GoToAsync(nameof(Pages.Sesiones.SesionGerente));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo regresar: {ex.Message}", "OK");
            }
        }
    }
}