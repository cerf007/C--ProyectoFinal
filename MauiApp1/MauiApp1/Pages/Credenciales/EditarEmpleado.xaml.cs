using Microsoft.Maui.Controls;
using MauiApp1.ViewModels.CredencialesVM;

namespace MauiApp1.Pages.Credenciales;

public partial class EditarEmpleado : ContentPage
{
    public EditarEmpleado()
    {
        InitializeComponent();
        BindingContext = new EditarEmpleadoVM();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Cargar empleados al mostrar la página
        var vm = BindingContext as EditarEmpleadoVM;
        if (vm != null)
        {
            _ = vm.LoadAsync();
        }
    }
}