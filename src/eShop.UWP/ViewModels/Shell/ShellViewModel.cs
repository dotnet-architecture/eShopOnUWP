using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Contacts;

using Microsoft.Practices.ServiceLocation;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using eShop.UWP.Models;
using eShop.UWP.Services;
using eShop.UWP.Activation;
using eShop.UWP.Authentication;

namespace eShop.UWP.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        private readonly CommonViewModel DefaultViewModel = new CommonViewModel();

        static public ShellViewModel Current { get; private set; }

        static public NavigationServiceEx NavigationService => ServiceLocator.Current.GetInstance<NavigationServiceEx>();

        public ShellViewModel()
        {
            Current = this;
        }

        private ObservableCollection<NavigationItemModel> _menuItems;
        public ObservableCollection<NavigationItemModel> MenuItems
        {
            get { return _menuItems ?? (_menuItems = new ObservableCollection<NavigationItemModel>(GetMenuItems())); }
            set { Set(ref _menuItems, value); }
        }

        private IShell Shell { get; set; }

        public FrameworkElement FrameView => NavigationService.Frame.Content as FrameworkElement;

        public CommonViewModel FrameViewModel => FrameView == null ? DefaultViewModel : (FrameView.DataContext as CommonViewModel) ?? DefaultViewModel;

        private Contact _userContact = new Contact();
        public Contact UserContact
        {
            get { return _userContact; }
            set { Set(ref _userContact, value); }
        }

        private object _selectedItem;
        public object SelectedItem
        {
            get { return _selectedItem; }
            set { SetSelectedItem(value, navigate: true); }
        }

        private string _query;
        public string Query
        {
            get { return _query; }
            set { Set(ref _query, value); }
        }

        private bool _isDisabled = false;
        public bool IsDisabled
        {
            get { return _isDisabled; }
            set { Set(ref _isDisabled, value); }
        }

        private double _disableOpacity = 1.0;
        public double DisableOpacity
        {
            get { return _disableOpacity; }
            set { Set(ref _disableOpacity, value); }
        }

        public ICommand ItemSelectedCommand => new RelayCommand<ItemClickEventArgs>(OnItemSelected);

        public ICommand SearchCommand => new RelayCommand<AutoSuggestBoxQuerySubmittedEventArgs>(NavigateToCatalogSearch);

        public ICommand LogoutCommand => new RelayCommand(OnLogout);

        public async void Initialize(IShell shell, ActivationState activationState)
        {
            Shell = shell;
            NavigationService.Frame = Shell.NavigationFrame;
            NavigationService.Navigated += OnNavigated;

            NavigationService.Navigate(activationState.ViewModel.ToString(), activationState.Parameter);

            UserContact = await GetUserContactAsync();
        }

        private IEnumerable<NavigationItemModel> GetMenuItems()
        {
            yield return new NavigationItemModel(Symbol.Shop, "Main Catalog", typeof(CatalogViewModel).FullName) { Execute = NavigateToCatalog };
            yield return new NavigationItemModel(0xEB05, "Statistics & Charts", typeof(StatisticsViewModel).FullName);
            yield return new NavigationItemModel(Symbol.Add, "Add New Item", typeof(ItemDetailViewModel).FullName) { Execute = NavigateToNewCatalogItem };
        }

        private void SetSelectedItem(object value, bool navigate)
        {
            _selectedItem = value;
            RaisePropertyChanged("SelectedItem");

            if (navigate)
            {
                OnItemSelected(value);
            }
        }

        private void OnItemSelected(object selectedItem)
        {
            if (selectedItem is NavigationItemModel navigationItem)
            {
                ExecuteItem(navigationItem);
            }
            else if (selectedItem is NavigationViewItem navigationViewItem)
            {
                if (navigationViewItem.Name == "SettingsNavPaneItem")
                {
                    ExecuteSettings();
                }
            }
        }

        private static void ExecuteItem(NavigationItemModel navigationItem)
        {
            if (navigationItem.Execute != null)
            {
                navigationItem.Execute(navigationItem);
            }
            else
            {
                NavigationService.Navigate(navigationItem.Key);
            }
        }

        private static void ExecuteSettings()
        {
            NavigationService.Navigate(typeof(SettingsViewModel).FullName);
        }

        private void NavigateToCatalog(NavigationItemModel navigationItem)
        {
            NavigationService.Navigate(navigationItem.Key, new CatalogState());
        }

        private void NavigateToCatalogSearch(AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!String.IsNullOrEmpty(args.QueryText))
            {
                NavigationService.Navigate(typeof(CatalogViewModel).FullName, new CatalogState(args.QueryText));
            }
        }

        private void NavigateToNewCatalogItem(NavigationItemModel navigationItem)
        {
            NavigationService.Navigate(navigationItem.Key, new ItemDetailState());
        }

        private async void OnLogout()
        {
            if (await DialogBox.ShowAsync("Confirm Logout", "Are you sure you want to logout?", "Ok", "Cancel"))
            {
                NavigationService.MainFrame.GoBack();
            }
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            var viewModelName = NavigationService.GetNameOfRegisteredPage(e.SourcePageType);
            var item = MenuItems.FirstOrDefault(r => r.Key == viewModelName);

            // TODO: Review
            if (viewModelName == typeof(ItemDetailViewModel).FullName)
            {
                var state = e.Parameter as ItemDetailState;
                if (state == null || state.Item != null)
                {
                    item = null;
                }
            }

            if (item != null)
            {
                SetSelectedItem(item, navigate: false);
            }
            else if (viewModelName == typeof(SettingsViewModel).FullName)
            {
                SetSelectedItem(Shell.SettingsItem, navigate: false);
            }
            else
            {
                // In order to unselect Settings, we need to select first a MenuItem
                SetSelectedItem(MenuItems.FirstOrDefault(), navigate: false);
                SetSelectedItem(null, navigate: false);
            }
            RaisePropertyChanged("FrameViewModel");
        }

        private async Task<Contact> GetUserContactAsync()
        {
            return await ContactHelper.CreateContactFromLogonUserAsync();
        }

        public void EnableView(bool enable)
        {
            IsDisabled = !enable;
            DisableOpacity = enable ? 1.0 : 0.75;
        }
    }
}
