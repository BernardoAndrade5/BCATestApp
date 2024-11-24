using System.Diagnostics;
using BCATestApp.Model;
using BCATestApp.Repositorys;
using BCATestApp.Services;
using BCATestApp.ViewModel;
using Moq;
using NUnit.Framework.Legacy;
using Syncfusion.Maui.Core.Carousel;

namespace BCAAppTests
{
    public class CarsViewModelTests
    {

        private Mock<CarsRepository> _mockCarsRepository;
        private Mock<NavigationService> _mockNavigationService;
        private CarsViewModel _carsViewModel;
        private Car _selectedCar;
        [SetUp]
        public void Setup()
        {
            _mockCarsRepository = new Mock<CarsRepository>();
            _mockNavigationService = new Mock<NavigationService>();
            _carsViewModel = new CarsViewModel(_mockCarsRepository.Object, _mockNavigationService.Object);
            _selectedCar = new Car { Make = "Ford", Model = "Fiesta", Favourite = false, EngineSize="1.8L", Fuel="petrol", AuctionDateTime= "2024/04/15 09:00:00" };
            _carsViewModel.SelectedCar = _selectedCar;
        }

        #region CurrentPageTests

        [Test]
        public void CurrentPage_SetValidValue_UpdatesCurrentPageAndTriggersDependencies()
        {
            const int maxPage = 10;

            _carsViewModel.MaxPage = maxPage; 

            _carsViewModel.CurrentPage = 5; 

            Assert.That(_carsViewModel.CurrentPage, Is.EqualTo(5));
            Assert.That(_carsViewModel.CurrentPageText, Is.Not.Null);
        }

        [Test]
        public void CurrentPage_SetValueAboveMaximum_ClampsToMaximum()
        {
            const int maxPage = 10;

            _carsViewModel.MaxPage = maxPage;

            _carsViewModel.CurrentPage = 15;

            Assert.That(_carsViewModel.CurrentPage, Is.EqualTo(maxPage));
            Assert.That(_carsViewModel.CurrentPageText, Is.Not.Null);
        }

        [Test]
        public void CurrentPage_SetValueUnderMinimum_ClampsToMinimum()
        {
            const int maxPage = 10;

            _carsViewModel.MaxPage = maxPage;

            _carsViewModel.CurrentPage = 0;

            Assert.That(_carsViewModel.CurrentPage, Is.EqualTo(1));
            Assert.That(_carsViewModel.CurrentPageText, Is.Not.Null);
        }
        #endregion

        #region ToggleFavouritesTest
        [Test]
        public void ToggleFavouriteCommand_WhenExecuted_ChangesIsFavouriteProperty()
        {
            var initialState = _carsViewModel.SelectedCar.Favourite;

            _carsViewModel.ToggleFavouriteCommand.Execute(null);

            Assert.That(_carsViewModel.SelectedCar.Favourite, Is.Not.EqualTo(initialState));
        }

        [Test]
        public void ToggleFavouriteCommand_WhenExecuted_MultipleTimes()
        {
            var initialState = _carsViewModel.SelectedCar.Favourite;

            _carsViewModel.ToggleFavouriteCommand.Execute(null);
            var firstToggleState = _carsViewModel.SelectedCar.Favourite;

            _carsViewModel.ToggleFavouriteCommand.Execute(null);
            var secondToggleState = _carsViewModel.SelectedCar.Favourite;

            Assert.That(firstToggleState, Is.Not.EqualTo(initialState));
            Assert.That(secondToggleState, Is.EqualTo(initialState));
        }
        #endregion

        #region CollectionViewPageUpCommand

        [Test]
        public void CollectionViewPageUpCommand_CurrentPage_Adds_WhenExecuted()
        {
            const int initialPage = 4;
            const int expectedPage = initialPage + 1;
            _carsViewModel.MaxPage = 10;

            _carsViewModel.CurrentPage = initialPage;

            _carsViewModel.CollectionViewPageUpCommand.Execute(null);

            Assert.That(_carsViewModel.CurrentPage, Is.EqualTo(expectedPage));
        }

        [Test]
        public void CollectionViewPageUpCommand_CurrentPage_NotGoOver_MaxPages()
        {
            const int initialPage = 4;
            _carsViewModel.MaxPage = 4;

            _carsViewModel.CurrentPage = initialPage;

            _carsViewModel.CollectionViewPageUpCommand.Execute(null);

            Assert.That(_carsViewModel.CurrentPage, Is.EqualTo(_carsViewModel.MaxPage));
        }
        #endregion

        #region CollectionViewPageDownCommand

        [Test]
        public void CollectionViewPageDownCommand_CurrentPage_Subtracts_WhenExecuted()
        {
            const int initialPage = 4;
            const int expectedPage = initialPage - 1;
            _carsViewModel.MaxPage = 10;

            _carsViewModel.CurrentPage = initialPage;

            _carsViewModel.CollectionViewPageDownCommand.Execute(null);

            Assert.That(_carsViewModel.CurrentPage, Is.EqualTo(expectedPage));
        }

        [Test]
        public void CollectionViewPageDownCommand_CurrentPage_NotGoUnder_MinPages()
        {
            const int initialPage = 1;
            _carsViewModel.MaxPage = 4;

            _carsViewModel.CurrentPage = initialPage;

            _carsViewModel.CollectionViewPageDownCommand.Execute(null);

            Assert.That(_carsViewModel.CurrentPage, Is.EqualTo(1));
        }
        #endregion

        #region SelectedCarBrand

        [Test]
        public void SelectedCarBrands_SetToAllBrands_ResetesBrandSelection()
        {
            _carsViewModel.SelectedCarBrand = "Toyota";

            _carsViewModel.SelectedCarBrand = "All Brands";

            Assert.That(_carsViewModel.IsBrandSelected, Is.False);
            Assert.That(_carsViewModel.SelectedCarModel, Is.EqualTo("All Models"));
        }

        [Test]
        public void SelectedCarBrands_SetToSelectedBrand()
        {
            _carsViewModel.SelectedCarBrand = "All Brands";

            _carsViewModel.SelectedCarBrand = "Toyota";

            Assert.That(_carsViewModel.IsBrandSelected, Is.True);
            Assert.That(_carsViewModel.SelectedCarModel, Is.EqualTo("All Models"));
        }
        #endregion
    }
}
