using eShop.UWP.ViewModels.Shell;
using eShop.UWP.Views.Base;
using Windows.UI.Xaml.Navigation;

namespace eShop.UWP.Views.Shell
{
    public sealed partial class ShellView : PageBase
    {
        private ShellViewModel ViewModel => DataContext as ShellViewModel;

        public ShellView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ViewModel.Initialize(shellFrame);
        }
    }
}
