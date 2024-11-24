using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCATestApp.Model;
using BCATestApp.Services;

namespace BCATestApp.Repositorys
{
    public interface ICarsRepository
    {
        Task<List<Car>> GetAllCarsCachedAsync();
        Task<List<Car>> GetCarsFilteredByBrandAsync(String make);
        Task<List<string>> GetBrandsListAsync();
        Task<List<string>> GetModelsPerBrandListAsync(String make);
    }
}
