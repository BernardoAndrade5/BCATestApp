using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCATestApp.Services
{
    public interface INavigationService
    {

        Task NavigateToAsync(string page);

        Task PopAsync();
    }
}
