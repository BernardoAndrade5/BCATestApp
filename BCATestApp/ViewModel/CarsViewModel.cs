﻿using BCATestApp.Model;
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
        public Command CombinedCommand { get;}
        public Command ApplyFiltersCommand { get; }
        public Command NavigateToPageCommand{ get; }
        public Command PopCurrentPageCommand { get; }
        public Command CollectionViewPageUpCommand { get; }
        public Command CollectionViewPageDownCommand { get; }


        private readonly CarsRepository _carRepository;
        private readonly NavigationService _navigationService;

        private string _selectedCarBrand = "";
        private string _selectedCarModel = "";
        private int _minStartingBid = 0;
        private int _maxStartingBid = 50000;
        private bool _isBrandSelected;
        private bool _isBrandChanged;
        private int PageSize = 20;
        private bool _isCollecitonViewButtonDownEnable  = false;
        public int _currentPage = 1;
        public int _maxPages = 0;

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
            CollectionViewPageUpCommand = new Command(HandleCollectionViewPageUp);
            CollectionViewPageDownCommand = new Command(HandleCollectionViewPageDown);
        }

        public string CurrentPageText => $"Page {CurrentPage} of {MaxPage}";

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
                    OnPropertyChanged();
                    ApplyFilters();
                    OnPropertyChanged(nameof(CurrentPageText));
                }
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

        private void GetMaxPageNumber(List<Car> filteredCars)
        {
            // Calculate total pages based on filteredCars.Count and PageSize
            MaxPage = (int)Math.Ceiling((double)filteredCars.Count / PageSize);

            CurrentPage = Math.Min(CurrentPage, MaxPage);
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


                GetMaxPageNumber(filteredCars);

                var currentPage = CurrentPage - 1;

                filteredCars.Sort((x, y) => x.StartingBid.CompareTo(y.StartingBid));

                filteredCars = filteredCars.Skip(PageSize * currentPage).Take(PageSize).ToList();
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
