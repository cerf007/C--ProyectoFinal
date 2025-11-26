namespace VariedSoftApp
{
    public partial class ReportsPage : ContentPage
    {
        public ReportsPage()
        {
            InitializeComponent();
        }

        // Métodos de los botones de reporte

        [Obsolete]
        private async void OnVentasClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Reporte", "Cargando reportes detallados de Ventas...", "OK");
            // Lógica para navegar a la vista de Reporte de Ventas
        }

        [Obsolete]
        private async void OnInventarioClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Reporte", "Cargando reportes de Inventario...", "OK");
            // Lógica para navegar a la vista de Reporte de Inventario
        }

        [Obsolete]
        private async void OnEmpleadosClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Reporte", "Cargando reportes de Rendimiento de Empleados...", "OK");
            // Lógica para navegar a la vista de Reporte de Empleados
        }

        [Obsolete]
        private async void OnCuentasClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Reporte", "Cargando reportes de Cuentas por Pagar/Cobrar...", "OK");
            // Lógica para navegar a la vista de Cuentas
        }
    }
}