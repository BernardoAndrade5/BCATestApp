using BCATestApp.View;
using BCATestApp.ViewModel;

namespace BCATestApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage(CarsViewModel carsViewModel)
        {
            InitializeComponent();
            BindingContext = carsViewModel;
        }
    }
}
