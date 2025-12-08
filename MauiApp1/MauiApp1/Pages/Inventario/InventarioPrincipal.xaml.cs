using Microsoft.Maui.Controls;
using MauiApp1.ViewModels.InventarioVM;

namespace MauiApp1.Pages.Inventario;

public partial class InventarioPrincipal : ContentPage
{
    public InventarioPrincipal()
    {
        InitializeComponent();
        BindingContext = new InventarioPrincipalVM();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var vm = BindingContext as InventarioPrincipalVM;
        if (vm != null)
        {
            _ = vm.LoadAsync();
        }
    }
}