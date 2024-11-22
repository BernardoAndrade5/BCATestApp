using BCATestApp.Model;
using BCATestApp.Repositorys;
using BCATestApp.Services;
using BCATestApp.View;
using CommunityToolkit.Maui.Views;
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
        public Command CombinedCommand { get;}
        public Command ApplyFiltersCommand { get; }
        public Command NavigateToPageCommand{ get; }
        public Command PopCurrentPageCommand { get; }

        private readonly CarsRepository _carRepository;
        private readonly NavigationService _navigationService;

        private string _selectedCarBrand = "";
        private string _selectedCarModel = "";
        private int _minStartingBid = 0;
        private int _maxStartingBid = 50000;
        private bool _isBrandSelected;
        private bool _isBrandChanged;

        public CarsViewModel(CarsRepository carsRepository, NavigationService navigationService)
        {
            Title = "Car Finder";
            _carRepository = carsRepository ?? throw new ArgumentNullException(nameof(carsRepository), "CarsRepository cannot be null");
            _navigationService = navigationService;
            Task.Run(async () =>
            {
                await GetAllCarBrandsAsync();
                await GetAllCars();
            });
            ApplyFiltersCommand = new Command(ApplyFilters);
            NavigateToPageCommand = new Command<string>(async (page) => await NavigateToPageAsync(page));
            PopCurrentPageCommand = new Command(async () => await PopCurrentPageAsync());
            CombinedCommand = new Command<string>(async (page) => 
                {
                    ApplyFilters();
                    await NavigateToPageAsync(page);
                });
        }

        public int MinStartingBid
        {
            get => _minStartingBid;
            set
            {
                if(_minStartingBid != value)
                {
                    _minStartingBid = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MaxStartingBid
        {
            get => _maxStartingBid;
            set
            {
                if (_maxStartingBid != value)
                {
                    _maxStartingBid = value;
                    OnPropertyChanged();
                }
            }
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
                    if (SelectedCarBrand == "All Brands")
                    {
                        IsBrandSelected = false;
                        SelectedCarModel = "All Models"; 
                    }
                    else
                    {
                        IsBrandSelected = true;
                        _isBrandChanged = true;
                        _ = GetAllCarModelsAsync(); 
                    }
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
;
                }
                else if (SelectedCarBrand == "All Brands")
                {
                    filteredCars = await _carRepository.GetAllCarsCachedAsync();
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

                if(MinStartingBid > 0.0 && MaxStartingBid < 50000)
                {
                    filteredCars = filteredCars.Where(Car => Car.StartingBid >= MinStartingBid && Car.StartingBid <= MaxStartingBid).ToList();
                }

                if(filteredCars.Count == 0)
                {
                    Debug.WriteLine("Filtered List is Empty");
                }

                Cars.Clear();
                filteredCars.Slice(20);
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

                if (brands.Count <= 3)
                {
                    await GetAllCarBrandsAsync();
                }

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

        private async Task NavigateToPageAsync(String page)
        {
            await _navigationService.NavigateToAsync(page);
        }

        private async Task PopCurrentPageAsync()
        {
            await _navigationService.PopAsync();
        }
    }
}
