using MauiApp1.ViewModels;
namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageVM vm)
        {
            InitializeComponent();
            BindingContext = vm;

        }
        
        
    }
}
