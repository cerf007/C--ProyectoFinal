using Microsoft.Maui.Controls;
using System;
using MauiApp1.ViewModels.InventarioVM;

namespace MauiApp1.Pages.Inventario;

public partial class EliminarProducto : ContentPage
{
    public EliminarProducto()
    {
        InitializeComponent();

        var vm = new EliminarProductoVM();
        BindingContext = vm;

        // Iniciar carga (fire-and-forget seguro porque VM maneja errores)
        _ = vm.CargarCommand.ExecuteAsync(null);
    }

    private async void OnRegresarClicked(object sender, EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync(nameof(Pages.Inventario.InventarioPrincipal));
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo regresar: {ex.Message}", "OK");
        }
    }
}