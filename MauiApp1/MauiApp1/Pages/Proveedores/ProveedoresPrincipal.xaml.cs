using Microsoft.Maui.Controls;
using MauiApp1.ViewModels.ProveedoresVM;
using System;

namespace MauiApp1.Pages.Proveedores;

public partial class ProveedoresPrincipal : ContentPage
{
    public ProveedoresPrincipal()
    {
        InitializeComponent();
        BindingContext = new ProveedoresPrincipalVM();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var vm = BindingContext as ProveedoresPrincipalVM;
        if (vm != null)
        {
            _ = vm.LoadAsync();
        }
    }
}