using BCATestApp.Model;
using BCATestApp.Repositorys;
using BCATestApp.Services;
using BCATestApp.View;
using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Windows.Input;

namespace BCATestApp.ViewModel
{
    public partial class CarsViewModel : BaseViewModel
    {
        public ObservableCollection<Car> Cars { get; } = new();
        public ObservableCollection<string> CarBrands { get; } = new();
        public ObservableCollection<string> CarModels { get; } = new();
        public ObservableCollection<string> SortingOptions {get; } = new();
        public ObservableCollection<int> PageSizeOptions { get; } = new();
        public Command ApplyFiltersCommand { get; }
        public Command NavigateToPageCommand{ get; }
        public Command PopCurrentPageCommand { get; }
        public Command CollectionViewPageUpCommand { get; }
        public Command CollectionViewPageDownCommand { get; }
        public Command NavigateToDetailPageCommand { get; }
        public Command ToggleFavouriteCommand { get; }

        private readonly CarsRepository _carRepository;
        private readonly NavigationService _navigationService;

        private string _selectedCarBrand = "";
        private string _selectedCarModel = "";
        private int _minStartingBid = 0;
        private int _maxStartingBid = 50000;
        private bool _isBrandSelected;
        private bool _isBrandChanged;
        private int _pageSizeSelected = 20;
        private bool _isCollecitonViewButtonDownEnable  = false;
        public int _currentPage = 1;
        public int _maxPages = 0;
        public string _sortingOptionSelected = "";
        private Car _selectedCar;
        private bool _onlyFavourites;

        public CarsViewModel(CarsRepository carsRepository, NavigationService navigationService)
        {
            _carRepository = carsRepository;
            _navigationService = navigationService;
            Cars.Clear();
            Task.Run(async () => await GetAllCarBrandsAsync());
            SortingOptionsHandler();
            PageSizeOptionsHandler();
            ApplyFiltersCommand = new Command(async () => await ApplyFilters());
            NavigateToPageCommand = new Command<string>(async (page) => await NavigateToPageAsync(page));
            PopCurrentPageCommand = new Command(async () => await PopCurrentPageAsync());
            CollectionViewPageUpCommand = new Command(HandleCollectionViewPageUp);
            CollectionViewPageDownCommand = new Command(HandleCollectionViewPageDown);
            NavigateToDetailPageCommand = new Command<Car>(async (selectedCar) => await HandleSelectedCarAsync(selectedCar));
            ToggleFavouriteCommand = new Command(ToggleFavourite);  
        }



        public string CurrentPageText => $"Page {CurrentPage} of {MaxPage}";

        public Car SelectedCar
        {
            get => _selectedCar;
            set
            {
                if (_selectedCar != value)
                {
                    _selectedCar = value;
                    Debug.WriteLine(SelectedCar);
                    OnPropertyChanged();
                }
            }
        }

        public string SortingOptionSelected
        {
            get => _sortingOptionSelected;
            set
            {
                if(_sortingOptionSelected != value)
                {
                    _sortingOptionSelected = value;
                    OnPropertyChanged();
                    UpdateFiltersForPageAsync();
                }
            }
        }

        public int PageSizeSelected
        {
            get => _pageSizeSelected;
            set
            {
                if (_pageSizeSelected != value)
                {
                    _pageSizeSelected = value;
                    OnPropertyChanged();
                    UpdateFiltersForPageAsync();
                }
            }
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

        public bool IsPagedownButtonEnable
        {
            get => _isCollecitonViewButtonDownEnable;
            set
            {
                if (_isCollecitonViewButtonDownEnable != value)
                {
                    _isCollecitonViewButtonDownEnable = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MaxPage
        {
            get => _maxPages;
            set
            {
                if (_maxPages != value)
                {
                    _maxPages = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CurrentPageText));
                }
            }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = Math.Max(1, Math.Min(value, MaxPage));

                    IsPagedownButtonEnable = _currentPage > 1;
                    Debug.WriteLine(IsPagedownButtonEnable);
                    OnPropertyChanged();
                    UpdateFiltersForPageAsync();
                    OnPropertyChanged(nameof(CurrentPageText));
                }
            }
        }

        public bool OnlyFavourites
        {
            get => _onlyFavourites;
            set
            {
                if(_onlyFavourites != value)
                {
                    _onlyFavourites = value;
                    OnPropertyChanged();
                }
            }
        }


        private void ToggleFavourite()
        {
            SelectedCar.Favourite = !SelectedCar.Favourite;
            Debug.WriteLine(SelectedCar.Favourite);
            OnPropertyChanged(nameof(SelectedCar));
        }

        private async Task HandleSelectedCarAsync(Car selectedCar)
        {
            if (selectedCar != null)
            {
                SelectedCar = selectedCar;
                await _navigationService.NavigateToAsync("CarDetailView");
            }
        }

        private void SortingOptionsHandler()
        {
            SortingOptions.Add("StartingBid");
            SortingOptions.Add("Make");
            SortingOptions.Add("Milage");
            SortingOptions.Add("AuctionDate");
        }

        private void PageSizeOptionsHandler()
        {
            PageSizeOptions.Add(20);
            PageSizeOptions.Add(40);
            PageSizeOptions.Add(60);
            PageSizeOptions.Add(80);
        }

        private IEnumerable<Car> SortCars (IEnumerable<Car> cars)
        {
            switch (SortingOptionSelected)
            {
                case "StartingBid":
                    return cars.OrderBy(car => car.StartingBid);

                case "Make":
                    return cars.OrderBy(car => car.Make);

                case "Milage":
                    return cars.OrderBy(car => car.Mileage);  

                case "AuctionDate":
                    return cars.OrderBy(car => car.AuctionDateTime);  

                default:
                    return cars.OrderBy(car => car.StartingBid);  
            }
        }

        private void HandleCollectionViewPageUp() {
            if (CurrentPage < MaxPage)
                CurrentPage++;
        }

        private void HandleCollectionViewPageDown()
        {
            if (CurrentPage > 1)
                CurrentPage--;
        }

        private void GetMaxPageNumber(int carsCount)
        {
            MaxPage = (int)Math.Ceiling((double)carsCount / PageSizeSelected);

            CurrentPage = Math.Min(CurrentPage, MaxPage);
        }

        private async void UpdateFiltersForPageAsync()
        {
            await ApplyFilters();
        }

        public async Task ApplyFilters()
        {
            IsLoading = true;
            try
            {
                Cars.Clear();
                var allCars = await Task.Run(() => _carRepository.GetAllCarsCachedAsync()); 
                var filteredCars = allCars.AsEnumerable();

                if (OnlyFavourites)
                {
                    filteredCars = filteredCars.Where(car => car.Favourite);
                }
                if (_isBrandChanged && !string.IsNullOrWhiteSpace(SelectedCarBrand) && SelectedCarBrand != "All Brands")
                {
                    filteredCars = filteredCars.Where(car => car.Make == SelectedCarBrand);
;
                }
                if (!string.IsNullOrWhiteSpace(SelectedCarModel) && SelectedCarModel != "All Models")
                {
                    filteredCars = filteredCars.Where(car => car.Model == SelectedCarModel);
                }
                if(MinStartingBid > 0.0 && MaxStartingBid < 50000)
                {
                    filteredCars = filteredCars.Where(car => car.StartingBid >= MinStartingBid && car.StartingBid <= MaxStartingBid);
                }


                filteredCars = SortCars(filteredCars);

                var currentPage = CurrentPage - 1;

                GetMaxPageNumber(filteredCars.Count());

                filteredCars = filteredCars
                    .Skip(PageSizeSelected * currentPage)
                    .Take(PageSizeSelected)
                    .ToList();

                await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        AddItemsToCollection(Cars, filteredCars);
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying filters: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
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

        private async Task NavigateToPageAsync(String page)
        {
            if(page == "CarCollectionView")
            {
                CurrentPage = 1;
                Cars.Clear();
                await _navigationService.NavigateToAsync(page);
                await ApplyFilters();
            }
            else
            {
                await _navigationService.NavigateToAsync(page);
            }
        }

        private async Task PopCurrentPageAsync()
        {
            await _navigationService.PopAsync();
        }
    }
}
