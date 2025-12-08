using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using MauiApp1.Modelos;
using MauiApp1.Servicios;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MauiApp1.ViewModels.Empleados
{
    internal class FacturaVM : ObservableObject
    {
        private readonly DatabaseService? _db;

        public string IDFactura { get => _idFactura; set => SetProperty(ref _idFactura, value); }
        public DateTime FechaEmision { get => _fechaEmision; set => SetProperty(ref _fechaEmision, value); }
        public string Hora { get => _hora; set => SetProperty(ref _hora, value); }
        public ObservableCollection<string> TiposPago { get => _tiposPago; set => SetProperty(ref _tiposPago, value); }
        public string TipoPago { get => _tipoPago; set => SetProperty(ref _tipoPago, value); }
        public string MontoTotal { get => _montoTotal; set => SetProperty(ref _montoTotal, value); }
        public string NumeroActualizacion { get => _numeroActualizacion; set => SetProperty(ref _numeroActualizacion, value); }
        public string IDCliente { get => _idCliente; set => SetProperty(ref _idCliente, value); }
        public string IDPersonal { get => _idPersonal; set => SetProperty(ref _idPersonal, value); }

        private string _idFactura = string.Empty;
        private DateTime _fechaEmision = DateTime.Now;
        private string _hora = string.Empty;
        private ObservableCollection<string> _tiposPago = new() { "Efectivo", "Tarjeta", "Transferencia" };
        private string _tipoPago = string.Empty;
        private string _montoTotal = string.Empty;
        private string _numeroActualizacion = "0";
        private string _idCliente = string.Empty;
        private string _idPersonal = string.Empty;

        public IAsyncRelayCommand GuardarCommand { get; }
        public IAsyncRelayCommand CancelarCommand { get; }
        public IAsyncRelayCommand ComandoRegresar { get; }

        public FacturaVM()
        {
            var mauiContext = Application.Current?.Handler?.MauiContext;
            _db = mauiContext?.Services.GetService(typeof(DatabaseService)) as DatabaseService;

            GuardarCommand = new AsyncRelayCommand(ExecuteGuardarAsync);
            CancelarCommand = new AsyncRelayCommand(ExecuteCancelarAsync);
            ComandoRegresar = new AsyncRelayCommand(ExecuteRegresarAsync);

            // Valor por defecto para TipoPago
            if (string.IsNullOrWhiteSpace(TipoPago) && TiposPago.Count > 0)
                TipoPago = TiposPago[0];
        }

        private async Task ExecuteGuardarAsync()
        {
            // Validaciones mínimas
            if (FechaEmision == default)
            {
                await Application.Current.MainPage.DisplayAlert("Validación", "Seleccione la fecha de emisión.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(MontoTotal) || !double.TryParse(MontoTotal, out var monto))
            {
                await Application.Current.MainPage.DisplayAlert("Validación", "Ingrese un monto total válido.", "OK");
                return;
            }

            try
            {
                if (_db == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Servicio de base de datos no disponible.", "OK");
                    return;
                }

                var factura = new Factura
                {
                    FechaEmision = FechaEmision,
                    Hora = Hora ?? string.Empty,
                    TipoPago = TipoPago ?? string.Empty,
                    TotalBruto = monto, // si quieres distinguir, adapta
                    MontoTotal = monto,
                    NumeroActualizacion = int.TryParse(NumeroActualizacion, out var n) ? n : 0,
                    IDCliente = int.TryParse(IDCliente, out var cl) ? cl : 0,
                    IDPersonal = int.TryParse(IDPersonal, out var p) ? p : 0
                };

                await _db.SaveFacturaAsync(factura);

                await Application.Current.MainPage.DisplayAlert("Éxito", "Factura guardada correctamente.", "OK");

                // Navegar a sesión de empleado (o regresar)
                try
                {
                    await Shell.Current.GoToAsync(nameof(Pages.Sesiones.SesionEmpleado));
                }
                catch
                {
                    // fallback
                    await Shell.Current.Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo guardar la factura: {ex.Message}", "OK");
            }
        }

        private Task ExecuteCancelarAsync()
        {
            // Limpiar campos (IDFactura sigue vacío hasta guardar)
            FechaEmision = DateTime.Now;
            Hora = string.Empty;
            TipoPago = TiposPago.Count > 0 ? TiposPago[0] : string.Empty;
            MontoTotal = string.Empty;
            NumeroActualizacion = "0";
            IDCliente = string.Empty;
            IDPersonal = string.Empty;
            return Task.CompletedTask;
        }

        private async Task ExecuteRegresarAsync()
        {
            try
            {
                if (Shell.Current.Navigation.NavigationStack.Count > 1)
                    await Shell.Current.Navigation.PopAsync();
                else
                    await Shell.Current.GoToAsync(nameof(Pages.Sesiones.SesionEmpleado));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo regresar: {ex.Message}", "OK");
            }
        }
    }
}