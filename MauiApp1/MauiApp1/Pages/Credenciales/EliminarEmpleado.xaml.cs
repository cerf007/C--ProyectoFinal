using Microsoft.Maui.Controls;
using MauiApp1.ViewModels.CredencialesVM;

namespace MauiApp1.Pages.Credenciales;

public partial class EliminarEmpleado : ContentPage
{
    public EliminarEmpleado()
    {
        InitializeComponent();
        BindingContext = new EliminarEmpleadoVM();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var vm = BindingContext as EliminarEmpleadoVM;
        if (vm != null)
        {
            _ = vm.LoadAsync();
        }
    }
}