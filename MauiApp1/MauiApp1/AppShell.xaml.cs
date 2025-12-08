using Microsoft.Maui.Controls;
using MauiApp1.Pages.Sesiones;
using MauiApp1.Pages.Credenciales;
using MauiApp1.Pages.Inventario;
using MauiApp1.Pages.Proveedores;

namespace MauiApp1
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Sesiones
            Routing.RegisterRoute(nameof(SesionGerente), typeof(SesionGerente));
            Routing.RegisterRoute(nameof(SesionEmpleado), typeof(SesionEmpleado));

            // Credenciales (Gerente)
            Routing.RegisterRoute(nameof(CredencialesPrincipal), typeof(CredencialesPrincipal));
            Routing.RegisterRoute(nameof(AgregarEmpleado), typeof(AgregarEmpleado));
            Routing.RegisterRoute(nameof(EditarEmpleado), typeof(EditarEmpleado));
            Routing.RegisterRoute(nameof(EliminarEmpleado), typeof(EliminarEmpleado));

            // Inventario (Gerente)
            Routing.RegisterRoute(nameof(InventarioPrincipal), typeof(InventarioPrincipal));
            Routing.RegisterRoute(nameof(AgregarProducto), typeof(AgregarProducto));
            Routing.RegisterRoute(nameof(EditarProducto), typeof(EditarProducto));
            Routing.RegisterRoute(nameof(EliminarProducto), typeof(EliminarProducto));

            // Proveedores (Gerente)
            Routing.RegisterRoute(nameof(ProveedoresPrincipal), typeof(ProveedoresPrincipal));
            Routing.RegisterRoute(nameof(AgregarProveedores), typeof(AgregarProveedores));
            Routing.RegisterRoute(nameof(EliminarProveedores), typeof(EliminarProveedores));

            //Generar Factura o Comprobar Existencia (Empleado)
            Routing.RegisterRoute(nameof(Pages.Empleados.Factura), typeof(Pages.Empleados.Factura));

            //Reportes (Gerente)
            Routing.RegisterRoute(nameof(Pages.Reportes.ReportesPrincipal), typeof(Pages.Reportes.ReportesPrincipal));
        }
    }
}