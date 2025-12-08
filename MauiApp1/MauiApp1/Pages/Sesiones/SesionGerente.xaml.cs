using Microsoft.Maui.Controls;
using MauiApp1.ViewModels.SesionesVM;
namespace MauiApp1.Pages.Sesiones;

public partial class SesionGerente : ContentPage
{
	public SesionGerente()
	{
		InitializeComponent();
		BindingContext = new SesionGerenteVM();
	}
}