using Microsoft.Maui.Controls;
using MauiApp1.ViewModels.CredencialesVM;

namespace MauiApp1.Pages.Credenciales;

public partial class AgregarEmpleado : ContentPage
{
    public AgregarEmpleado()
    {
        InitializeComponent();
        BindingContext = new AgregarEmpleadoVM();
    }
}