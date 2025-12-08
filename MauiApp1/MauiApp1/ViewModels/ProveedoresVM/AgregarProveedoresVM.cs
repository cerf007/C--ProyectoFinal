using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using MauiApp1.Modelos;
using MauiApp1.Servicios;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MauiApp1.ViewModels.ProveedoresVM
{
    internal class AgregarProveedoresVM : ObservableObject
    {
        private readonly DatabaseService? _db;

        public string EmpresaNombre { get => _empresaNombre; set => SetProperty(ref _empresaNombre, value); }
        public string RFC { get => _rfc; set => SetProperty(ref _rfc, value); }
        public string NombreContacto { get => _nombreContacto; set => SetProperty(ref _nombreContacto, value); }
        public string Email { get => _email; set => SetProperty(ref _email, value); }
        public string Telefono { get => _telefono; set => SetProperty(ref _telefono, value); }
        public string Categoria { get => _categoria; set => SetProperty(ref _categoria, value); }
        public string Direccion { get => _direccion; set => SetProperty(ref _direccion, value); }
        public string Calificacion { get => _calificacion; set => SetProperty(ref _calificacion, value); }

        private string _empresaNombre = string.Empty;
        private string _rfc = string.Empty;
        private string _nombreContacto = string.Empty;
        private string _email = string.Empty;
        private string _telefono = string.Empty;
        private string _categoria = string.Empty;
        private string _direccion = string.Empty;
        private string _calificacion = "5";

        public IAsyncRelayCommand GuardarCommand { get; }
        public IAsyncRelayCommand CancelarCommand { get; }
        public IAsyncRelayCommand RegresarCommand { get; }

        public AgregarProveedoresVM()
        {
            var mauiContext = Application.Current?.Handler?.MauiContext;
            _db = mauiContext?.Services.GetService(typeof(DatabaseService)) as DatabaseService;

            GuardarCommand = new AsyncRelayCommand(ExecuteGuardarAsync);
            CancelarCommand = new AsyncRelayCommand(ExecuteCancelarAsync);
            RegresarCommand = new AsyncRelayCommand(ExecuteRegresarAsync);
        }

        private bool EmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        private async Task<bool> ConfirmarRFCInusualAsync(string rfc)
        {
            // Si RFC tiene formato muy corto, pedir confirmación (ejemplo heurístico)
            if (!string.IsNullOrWhiteSpace(rfc) && rfc.Trim().Length < 8)
            {
                return await Application.Current.MainPage.DisplayAlert(
                    "Validación",
                    "El RFC parece corto. ¿Desea continuar?",
                    "Continuar",
                    "Cancelar");
            }
            return true;
        }

        private async Task ExecuteGuardarAsync()
        {
            // Validaciones mínimas
            if (string.IsNullOrWhiteSpace(EmpresaNombre) || EmpresaNombre.Trim().Length < 2)
            {
                await Application.Current.MainPage.DisplayAlert("Validación", "Ingrese el nombre de la empresa (mín. 2 caracteres).", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(RFC) || RFC.Trim().Length < 4)
            {
                await Application.Current.MainPage.DisplayAlert("Validación", "Ingrese un RFC válido (mín. 4 caracteres).", "OK");
                return;
            }

            if (!string.IsNullOrWhiteSpace(Email) && !EmailValido(Email))
            {
                await Application.Current.MainPage.DisplayAlert("Validación", "Ingrese un correo válido o deje el campo vacío.", "OK");
                return;
            }

            try
            {
                if (_db == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Servicio de base de datos no disponible.", "OK");
                    return;
                }

                // Confirmar RFC inusual
                var continuar = await ConfirmarRFCInusualAsync(RFC);
                if (!continuar) return;

                // Evitar duplicados por nombre de empresa (case-insensitive)
                var lista = await _db.GetAllProveedoresAsync();
                var dup = lista.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.Nombre)
                                                    && p.Nombre.Trim().Equals(EmpresaNombre.Trim(), StringComparison.OrdinalIgnoreCase));
                if (dup != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Validación", "Ya existe un proveedor con ese nombre.", "OK");
                    return;
                }

                // Mapear a modelo Proveedor
                var proveedor = new Proveedor
                {
                    Nombre = EmpresaNombre.Trim(),
                    RFC = RFC.Trim(),
                    NombreContacto = NombreContacto?.Trim() ?? string.Empty,
                    Email = Email?.Trim() ?? string.Empty,
                    Telefono = Telefono?.Trim() ?? string.Empty,
                    Categoria = Categoria?.Trim() ?? string.Empty,
                    Direccion = Direccion?.Trim() ?? string.Empty
                };

                // Calificación: intentar parsear
                if (int.TryParse(Calificacion, out var cal))
                    proveedor.Calificacion = Math.Clamp(cal, 1, 5);
                else
                    proveedor.Calificacion = 5;

                await _db.SaveProveedorAsync(proveedor);

                await Application.Current.MainPage.DisplayAlert("Éxito", "Proveedor guardado correctamente.", "OK");

                // Volver a la pantalla principal de proveedores
                await Shell.Current.GoToAsync(nameof(Pages.Proveedores.ProveedoresPrincipal));
            }
            catch (Exception ex)
            {
                var msg = ex.Message ?? "Error interno";
                if (msg.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase))
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo guardar: ya existe un proveedor con ese identificador.", "OK");
                else
                    await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo guardar el proveedor: {msg}", "OK");
            }
        }

        private Task ExecuteCancelarAsync()
        {
            // Limpiar campos
            EmpresaNombre = string.Empty;
            RFC = string.Empty;
            NombreContacto = string.Empty;
            Email = string.Empty;
            Telefono = string.Empty;
            Categoria = string.Empty;
            Direccion = string.Empty;
            Calificacion = "5";
            return Task.CompletedTask;
        }

        private async Task ExecuteRegresarAsync()
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(Pages.Proveedores.ProveedoresPrincipal));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo regresar: {ex.Message}", "OK");
            }
        }
    }
}