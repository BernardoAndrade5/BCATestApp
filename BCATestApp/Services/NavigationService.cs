using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCATestApp.Services
{
    public class NavigationService : INavigationService
    {
        public async Task NavigateToAsync(String page)
        {
            try
            {
                await Shell.Current.GoToAsync(page);
            }
            catch (Exception ex) {
                Console.WriteLine($"Navigation failed: {ex.Message}");
            }

        }

        public async Task PopAsync()
        {
            try
            {
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Pop failed: {ex.Message}");
            }
        }
    }
}
