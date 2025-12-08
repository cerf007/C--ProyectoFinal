using Microsoft.Maui.Controls;
using MauiApp1.ViewModels.ProveedoresVM;

namespace MauiApp1.Pages.Proveedores;

public partial class AgregarProveedores : ContentPage
{
    public AgregarProveedores()
    {
        InitializeComponent();
        BindingContext = new AgregarProveedoresVM();
    }
}