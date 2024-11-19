using BCATestApp.Model;
using BCATestApp.Repositorys;
using BCATestApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCATestApp.ViewModel
{
    public partial class CarsViewModel : BaseViewModel
    {
        public ObservableCollection<Car> Cars { get; } = new();
        public ObservableCollection<string> CarBrands { get; } = new();
        private readonly CarsRepository _carRepository;
        private string _selectedCarBrand = "";
        public string SelectedCarBrand
        {
            get => _selectedCarBrand;
            set
            {
                if (_selectedCarBrand != value)
                {
                    _selectedCarBrand = value;
                    OnPropertyChanged(); 
                    if(_selectedCarBrand == "All Brands")
                    {
                        Task.Run(async () => await GetAllCars());
                    }
                    else Task.Run(async () => await GetCarsFilteredByMakeAsync(value));
                }
            }
        }

        public CarsViewModel(CarsRepository carsRepository)
        {
            Title = "Car Finder";
            _carRepository = carsRepository ?? throw new ArgumentNullException(nameof(carsRepository), "CarsRepository cannot be null");
            Task.Run(async () =>
            {
                await GetAllCarBrands();
                await GetAllCars();
            });
        }

        async Task GetCarsFilteredByMakeAsync(String make)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                Cars.Clear();

                var cars = await _carRepository.GetCarsFilteredByBrandAsync(make);

                foreach (var car in cars)
                {
                    Cars.Add(car);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to load car brands: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task GetAllCarBrands()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                CarBrands.Clear();

                var brands = await _carRepository.GetBrandsListAsync();

                foreach (var brand in brands)
                {
                    CarBrands.Add(brand);
                }
                CarBrands.Insert(0, "All Brands");
                SelectedCarBrand = "All Brands";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to load car brands: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task GetAllCars()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                var cars = await _carRepository.GetAllCarsCachedAsync();

                if (Cars.Count != 0)
                    Cars.Clear();

                foreach (var car in cars) {
                    Cars.Add(car);
                }
            }

            catch (Exception)
            {
                Debug.WriteLine($"Unable to get Cars");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
