using BCATestApp.View;

namespace BCATestApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(CarCollectionView), typeof(CarCollectionView));
        }
    }
}
