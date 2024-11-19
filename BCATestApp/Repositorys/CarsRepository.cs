using BCATestApp.Model;
using BCATestApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCATestApp.Repositorys
{
    public class CarsRepository
    {
        private readonly CarService _carService;
        private List<Car>? _cachedCars;

        public CarsRepository(CarService carService) { 
            _carService = carService;
            _cachedCars = null;
        }

        public async Task<List<Car>> GetAllCarsCachedAsync()
        {
            if (_cachedCars == null)
            {
                _cachedCars = await _carService.GetCars();
            }
            return _cachedCars;
        }

        public async Task<List<Car>> GetCarsFilteredByBrandAsync(String make) 
        {
            var carList = await GetAllCarsCachedAsync();
            return carList.Where(car => car.Make.Equals(make, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public async Task<List<string>> GetBrandsListAsync()
        {
            var carList = await GetAllCarsCachedAsync();
            return carList.Select(car => car.Make).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }
    }
}
