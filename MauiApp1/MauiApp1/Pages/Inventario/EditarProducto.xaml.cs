using Microsoft.Maui.Controls;
using System;
using MauiApp1.ViewModels.InventarioVM;

namespace MauiApp1.Pages.Inventario;

public partial class EditarProducto : ContentPage
{
    public EditarProducto()
    {
        InitializeComponent();

        // Asignar ViewModel y cargar datos
        var vm = new EditarProductoVM();
        BindingContext = vm;

        // Llamada segura a la carga inicial
        _ = vm.CargarCommand.ExecuteAsync(null);
    }

    private async void OnRegresarClicked(object sender, EventArgs e)
    {
        try
        {
            // Navegación refactor-safe (asegúrate que la ruta está registrada en AppShell)
            await Shell.Current.GoToAsync(nameof(InventarioPrincipal));
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo regresar: {ex.Message}", "OK");
        }
    }
}