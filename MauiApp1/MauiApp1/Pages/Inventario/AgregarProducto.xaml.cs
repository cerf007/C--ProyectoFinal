using MauiApp1.ViewModels.InventarioVM;
using Microsoft.Maui.Controls;

namespace MauiApp1.Pages.Inventario;

public partial class AgregarProducto : ContentPage
{
    public AgregarProducto()
    {
        InitializeComponent();

        // Asignamos el ViewModel como BindingContext para que los bindings en XAML funcionen.
        BindingContext = new AgregarProductoVM();
    }
}