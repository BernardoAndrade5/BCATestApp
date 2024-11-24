using BCATestApp.Repositorys;
using BCATestApp.Services;
using BCATestApp.ViewModel;
using Moq;

namespace BCAAppTests
{
    public class CarsViewModelTests
    {

        private Mock<CarService> _mockCarService;
        private Mock<CarsRepository> _mockCarsRepository;
        private Mock<NavigationService> _mockNavigationService;
        private CarsViewModel _carsViewModel;
        [SetUp]
        public void Setup()
        {
            _mockCarService = new Mock<CarService>();
            _mockCarsRepository = new Mock<CarsRepository>(_mockCarService.Object);
            _mockNavigationService = new Mock<NavigationService>();
            _carsViewModel = new CarsViewModel(_mockCarsRepository.Object, _mockNavigationService.Object);
        }

        [Test]
        public void Handle_Collection_View_Page_Up()
        {
        }
    }
}
