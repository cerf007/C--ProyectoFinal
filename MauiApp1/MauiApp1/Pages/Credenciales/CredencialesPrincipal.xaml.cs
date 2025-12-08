using Microsoft.Maui.Controls;
using MauiApp1.ViewModels.CredencialesVM;

namespace MauiApp1.Pages.Credenciales;

public partial class CredencialesPrincipal : ContentPage
{
    public CredencialesPrincipal()
    {
        InitializeComponent();
        BindingContext = new CredencialesPrincipalVM();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var vm = BindingContext as CredencialesPrincipalVM;
        if (vm != null)
        {
            _ = vm.LoadAsync();
        }
    }
}