using System;
using System.Threading.Tasks;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace eShop.UWP
{
    public class DeferredNavigation
    {
        private bool _isBusy = false;

        public DeferredNavigation(Frame frame)
        {
            Frame = frame;
            Frame.Navigating += OnFrameNavigating;
        }

        public Frame Frame { get; }

        public Func<NavigatingCancelEventArgs, Task> OnNavigating { get; set; }

        private void OnFrameNavigating(object sender, NavigatingCancelEventArgs e)
        {
            e.Cancel = true;
            if (!_isBusy)
            {
                HandleNavigationCancel(e);
            }
        }

        private async void HandleNavigationCancel(NavigatingCancelEventArgs e)
        {
            _isBusy = true;
            if (OnNavigating != null)
            {
                await OnNavigating(e);
                if (!e.Cancel)
                {
                    switch (e.NavigationMode)
                    {
                        case NavigationMode.New:
                        case NavigationMode.Refresh:
                            Frame.Navigating -= OnFrameNavigating;
                            Frame.Navigate(e.SourcePageType, e.Parameter);
                            break;
                        case NavigationMode.Back:
                            Frame.Navigating -= OnFrameNavigating;
                            Frame.GoBack();
                            break;
                        default:
                            break;
                    }
                }
            }
            _isBusy = false;
        }
    }
}
