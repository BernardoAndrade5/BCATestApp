using BCATestApp.Model;
using BCATestApp.Repositorys;
using BCATestApp.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace BCATestApp.ViewModel
{
    public partial class CarsViewModel : BaseViewModel
    {
        public ObservableCollection<Car> Cars { get; } = new();
        public ObservableCollection<string> CarBrands { get; } = new();
        public ObservableCollection<string> CarModels { get; } = new();

        private readonly CarsRepository _carRepository;

        private string _selectedCarBrand = "";
        private string _selectedCarModel = "";
        private bool _isBrandSelected;
        private bool _isBrandChanged;

        public CarsViewModel(CarsRepository carsRepository)
        {
            Title = "Car Finder";
            _carRepository = carsRepository ?? throw new ArgumentNullException(nameof(carsRepository), "CarsRepository cannot be null");
            Task.Run(async () =>
            {
                await GetAllCarBrandsAsync();
                await GetAllCars();
            });
        }


        public string SelectedCarBrand
        {
            get => _selectedCarBrand;
            set
            {
                if (_selectedCarBrand != value)
                {
                    _selectedCarBrand = value;
                    OnPropertyChanged();
                    _isBrandChanged = true;
                    ApplyFilters();
                }
            }
        }

        public string SelectedCarModel
        {
            get => _selectedCarModel;
            set
            {
                if (_selectedCarModel != value)
                {
                    _selectedCarModel = value;
                    OnPropertyChanged();
                    ApplyFilters();
                }
            }
        }

        public bool IsBrandSelected
        {
            get => _isBrandSelected;
            set
            {
                if (_isBrandSelected != value)
                {
                    _isBrandSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        private async void ApplyFilters()
        {
            try
            {
                var filteredCars = await _carRepository.GetAllCarsCachedAsync();

                if (_isBrandChanged && !string.IsNullOrWhiteSpace(SelectedCarBrand) && SelectedCarBrand != "All Brands")
                {
                    filteredCars = filteredCars.Where(car => car.Make == SelectedCarBrand).ToList();
                    IsBrandSelected = true;
                    await GetAllCarModelsAsync();
                    _isBrandChanged = false;
;
                }
                else if (SelectedCarBrand == "All Brands")
                {
                    filteredCars = await _carRepository.GetAllCarsCachedAsync();
                    IsBrandSelected = false;
                    SelectedCarModel = "All Models";
                }
                else
                {
                    filteredCars = filteredCars.ToList();
                }

                if (!string.IsNullOrWhiteSpace(SelectedCarModel) && SelectedCarModel != "All Models")
                {
                    filteredCars = filteredCars.Where(car => car.Model == SelectedCarModel).ToList();
                }

                else if (SelectedCarBrand != "All Brands")
                {
                    filteredCars = filteredCars.Where(car => car.Make == SelectedCarBrand).ToList();
                }

                Cars.Clear();
                AddItemsToCollection(Cars, filteredCars);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying filters: {ex.Message}");
            }
        }


        private async Task GetAllCarBrandsAsync()
        {
            await ExecuteWithBusyCheck(async () =>
            {
                CarBrands.Clear();

                var brands = await _carRepository.GetBrandsListAsync();

                AddItemsToCollection(CarBrands, brands);
                CarBrands.Insert(0, "All Brands");

                SelectedCarBrand = "All Brands"; 
            });
        }

        private async Task GetAllCarModelsAsync()
        {
            await ExecuteWithBusyCheck(async () =>
            {

                CarModels.Clear();

                try
                {
                    var models = await _carRepository.GetModelsPerBrandListAsync(SelectedCarBrand);
                    AddItemsToCollection(CarModels, models);

                    CarModels.Insert(0, "All Models");
                    SelectedCarModel = "All Models";
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error fetching models for brand {SelectedCarBrand}: {ex.Message}");
                }
            });
        }

        private async Task GetAllCars()
        {
            await ExecuteWithBusyCheck(async () =>
            {
                Cars.Clear();
                var allCars = await _carRepository.GetAllCarsCachedAsync();
                AddItemsToCollection(Cars, allCars);
            });
        }
    }
}
