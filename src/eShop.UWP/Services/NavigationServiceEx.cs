using System;
using System.Linq;
using System.Collections.Generic;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;

using eShop.UWP.Helpers;

namespace eShop.UWP.Services
{
    public class NavigationServiceEx
    {
        public event NavigatingCancelEventHandler Navigating;
        public event NavigatedEventHandler Navigated;
        public event NavigationStoppedEventHandler NavigationStopped;
        public event NavigationFailedEventHandler NavigationFailed;

        private readonly Dictionary<string, Type> _pages = new Dictionary<string, Type>();

        private Frame _frame;
        public Frame Frame
        {
            get
            {
                if (_frame == null)
                {
                    _frame = Window.Current.Content as Frame;
                    RegisterFrameEvents();
                }
                return _frame;
            }
            set
            {
                UnregisterFrameEvents();
                _frame = value;
                RegisterFrameEvents();
            }
        }

        public bool CanGoBack => Frame.CanGoBack;

        public bool CanGoForward => Frame.CanGoForward;

        public void GoBack() => Frame.GoBack();

        public void GoForward() => Frame.GoForward();

        public void Configure(string key, Type pageType)
        {
            lock (_pages)
            {
                if (_pages.ContainsKey(key))
                {
                    throw new ArgumentException(string.Format("ExceptionNavigationServiceExKeyIsInNavigationService".GetLocalized(), key));
                }

                if (_pages.Any(p => p.Value == pageType))
                {
                    throw new ArgumentException(string.Format("ExceptionNavigationServiceExTypeAlreadyConfigured".GetLocalized(), _pages.First(p => p.Value == pageType).Key));
                }

                _pages.Add(key, pageType);
            }
        }

        public bool Navigate(string pageKey, object parameter = null, NavigationTransitionInfo infoOverride = null)
        {
            Type page;
            lock (_pages)
            {
                if (!_pages.TryGetValue(pageKey, out page))
                {
                    throw new ArgumentException($"Page not found: {pageKey}. Did you forget to call NavigationService.Configure?", "pageKey");
                }
            }
            var navigationResult = Frame.Navigate(page, parameter, infoOverride);
            return navigationResult;
        }

        public string GetNameOfRegisteredPage(Type page)
        {
            lock (_pages)
            {
                return _pages.FirstOrDefault(p => p.Value == page).Key;
            }
        }

        private void RegisterFrameEvents()
        {
            if (_frame != null)
            {
                _frame.Navigating += OnNavigating;
                _frame.Navigated += OnNavigated;
                _frame.NavigationStopped += OnNavigationStopped;
                _frame.NavigationFailed += OnNavigationFailed;
            }
        }

        private void UnregisterFrameEvents()
        {
            if (_frame != null)
            {
                _frame.Navigating -= OnNavigating;
                _frame.Navigated -= OnNavigated;
                _frame.NavigationStopped -= OnNavigationStopped;
                _frame.NavigationFailed -= OnNavigationFailed;
            }
        }

        private void OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            Navigating?.Invoke(sender, e);
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            Navigated?.Invoke(sender, e);
        }

        private void OnNavigationStopped(object sender, NavigationEventArgs e)
        {
            NavigationStopped?.Invoke(sender, e);
        }

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            NavigationFailed?.Invoke(sender, e);
        }
    }
}
