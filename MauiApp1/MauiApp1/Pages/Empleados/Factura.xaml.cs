using Microsoft.Maui.Controls;
using MauiApp1.ViewModels.Empleados;

namespace MauiApp1.Pages.Empleados;

public partial class Factura : ContentPage
{
    public Factura()
    {
        InitializeComponent();
        BindingContext = new FacturaVM();
    }
}