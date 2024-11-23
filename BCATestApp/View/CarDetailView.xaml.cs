using BCATestApp.ViewModel;

namespace BCATestApp.View;

public partial class CarDetailView : ContentPage
{
	public CarDetailView(CarsViewModel carsViewModel)
    {
		InitializeComponent();
        BindingContext = carsViewModel;
    }
}