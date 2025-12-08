using Microsoft.Maui.Controls;
using MauiApp1.ViewModels.SesionesVM;

namespace MauiApp1.Pages.Sesiones;

public partial class SesionEmpleado : ContentPage
{
    public SesionEmpleado()
    {
        InitializeComponent();
        BindingContext = new SesionEmpleadoVM();
    }
}