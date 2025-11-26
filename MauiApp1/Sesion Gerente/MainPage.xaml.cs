using System;
using Microsoft.Maui.Controls;

namespace Sesion_Gerente
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        // Elimina este método para evitar ambigüedad con el generado por XAML.
        // private void InitializeComponent()
        // {
        //     throw new NotImplementedException();
        // }

        // Hacer públicos los manejadores evita problemas de visibilidad con el XAML AOT/XamlC
        public async void OnReportesClicked(object sender, EventArgs e)
        {
            await DisplayAlertAsync("Reportes", "Abrir sección de reportes.", "OK");
        }

        public async void OnProveedoresClicked(object sender, EventArgs e)
        {
            await DisplayAlertAsync("Proveedores", "Abrir sección de proveedores.", "OK");
        }

        public async void OnBasesDatosClicked(object sender, EventArgs e)
        {
            await DisplayAlertAsync("Bases de datos", "Abrir sección de bases de datos.", "OK");
        }

        public async void OnCuentasClicked(object sender, EventArgs e)
        {
            await DisplayAlertAsync("Cuentas", "Abrir sección de administración de cuentas.", "OK");
        }
    }
}
