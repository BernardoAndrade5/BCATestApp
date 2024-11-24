using BCATestApp.ViewModel;

namespace BCATestApp.View;

public partial class CarCollectionView : ContentPage
{
	public CarCollectionView(CarsViewModel carsViewModel)
	{
		InitializeComponent();
		BindingContext = carsViewModel;
	}
}