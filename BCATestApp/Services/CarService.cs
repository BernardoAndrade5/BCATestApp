using BCATestApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BCATestApp.Services
{
    public class CarService
    {
        List<Car> carList = new();

        public List<Car>? GetCarList()
        {
            return carList;
        }

        public async Task<List<Car>> GetCars()
        {
            carList ??= new List<Car>();

            if (carList.Any())
                return carList;

            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("vehicles_dataset.json");
                using var reader = new StreamReader(stream);
                var contents = await reader.ReadToEndAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true 
                };

                var carsFromJson = JsonSerializer.Deserialize<List<Car>>(contents, options);

                if (carsFromJson != null)
                    carList.AddRange(carsFromJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading or parsing JSON file: {ex.Message}");
            }

            return carList;
        }
    }
}
