using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace eShop.UWP.Services
{
    public class NavigationServiceEx
    {
        private readonly Dictionary<string, Type> _pages = new Dictionary<string, Type>();

        private Frame _frame;

        public Frame Frame
        {
            get
            {
                if (_frame == null)
                {
                    _frame = Window.Current.Content as Frame;
                }

                return _frame;
            }
            set
            {
                if(_frame != null)
                {
                    _frame.Navigated -= OnFrameNavigated;
                }
                if (value != null)
                {
                    value.Navigated += OnFrameNavigated;
                }
                _frame = value;
            }
        }

        public bool CanGoBack => Frame.CanGoBack;
        public bool CanGoForward => Frame.CanGoForward;

        public void GoBack() => Frame.GoBack();
        public void GoForward() => Frame.GoForward();

        public bool Navigate(string pageKey, object parameter = null, NavigationTransitionInfo infoOverride = null)
        {
            lock (_pages)
            {
                if (!_pages.ContainsKey(pageKey))
                {
                    throw new ArgumentException($"Page not found: {pageKey}. Did you forget to call NavigationService.Configure?", "pageKey");
                }
                var navigationResult = Frame.Navigate(_pages[pageKey], parameter, infoOverride);
                return navigationResult;
            }
        }

        public void Configure(string key, Type pageType)
        {
            lock (_pages)
            {
                if (_pages.ContainsKey(key))
                {
                    throw new ArgumentException($"The key {key} is already configured in NavigationService");
                }

                if (_pages.Any(p => p.Value == pageType))
                {
                    throw new ArgumentException($"This type is already configured with key {_pages.First(p => p.Value == pageType).Key}");
                }

                _pages.Add(key, pageType);
            }
        }

        public string GetNameOfRegisteredPage(Type page)
        {
            lock (_pages)
            {
                if (_pages.ContainsValue(page))
                {
                    return _pages.FirstOrDefault(p => p.Value == page).Key;
                }
                else
                {
                    throw new ArgumentException($"The page '{page.Name}' is unknown by the NavigationService");
                }
            }
        }

        public void CleanBackStack()
        {
            lock (_pages)
            {
                Frame.BackStack.Clear();
                SetBackButtonVisibility();
            }
        }

        private void OnFrameNavigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            SetBackButtonVisibility();
        }

        private void SetBackButtonVisibility()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = CanGoBack ?
                            AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }
    }
}
