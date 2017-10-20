using System;
using System.Windows.Input;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.ApplicationModel;

using GalaSoft.MvvmLight.Command;

using eShop.UWP.Services;
using eShop.UWP.ViewModels.Base;
using eShop.UWP.ViewModels.Catalog;
using eShop.UWP.ViewModels.Shell;

namespace eShop.UWP.ViewModels
{
    public class SettingsViewModel : CustomViewModelBase
    {
        public string DisplayName => Package.Current.DisplayName;

        public string Version
        {
            get
            {
                var ver = Package.Current.Id.Version;
                return $"Version {ver.Major}.{ver.Minor}.{ver.Build}.{ver.Revision}";
            }
        }

        public DataProviderType DataProvider => IsLocalProvider ? DataProviderType.Local : DataProviderType.REST;

        public bool HasChanges => DataProvider != AppSettings.Current.DataProvider || ServiceUrl != AppSettings.Current.ServiceUrl;

        private bool _isLocalProvider;
        public bool IsLocalProvider
        {
            get { return _isLocalProvider; }
            set { Set(ref _isLocalProvider, value); }
        }

        private bool _isRESTProvider;
        public bool IsRESTProvider
        {
            get { return _isRESTProvider; }
            set { Set(ref _isRESTProvider, value); }
        }

        private string _serviceUrl;
        public string ServiceUrl
        {
            get { return _serviceUrl; }
            set { Set(ref _serviceUrl, value); }
        }

        public Visibility CancelVisibility => NavigationService.CanGoBack ? Visibility.Visible : Visibility.Collapsed;

        private bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { Set(ref _isBusy, value); base.RaisePropertyChanged("IsIdle"); }
        }

        public bool IsIdle => !IsBusy;

        public ICommand ValidateConnectionCommand => new RelayCommand(OnValidateConnection);
        private async void OnValidateConnection()
        {
            var result = await ValidateAsync();
            await DialogBox.ShowAsync(result);
        }

        public ICommand SaveCommand => new RelayCommand(OnSave);
        private async void OnSave()
        {
            var result = await ValidateAndApplyChangesAsync();
            if (result.IsOk)
            {
                if (NavigationService.Frame == Window.Current.Content)
                {
                    NavigationService.Navigate(typeof(ShellViewModel).FullName);
                }
                else
                {
                    NavigationService.Navigate(typeof(CatalogViewModel).FullName);
                }
            }
        }

        public ICommand CancelCommand => new RelayCommand(OnCancel);
        private void OnCancel()
        {
            NavigationService.GoBack();
        }

        public void Initialize()
        {
            IsLocalProvider = AppSettings.Current.DataProvider == DataProviderType.Local;
            IsRESTProvider = AppSettings.Current.DataProvider == DataProviderType.REST;
            ServiceUrl = AppSettings.Current.ServiceUrl;
        }

        public async Task<Result> ValidateAndApplyChangesAsync()
        {
            var result = await ValidateAsync();
            if (result.IsOk)
            {
                ApplyChanges();
            }
            else
            {
                await DialogBox.ShowAsync(result);
            }
            return result;
        }

        public async Task<Result> ValidateAsync()
        {
            if (IsRESTProvider)
            {
                return await ValidateRESTAsync();
            }
            return Result.Ok();
        }

        private async Task<Result> ValidateRESTAsync()
        {
            if (String.IsNullOrEmpty(ServiceUrl))
            {
                return Result.Error("Empty address", "Please, enter a valid service url.");
            }

            if (!Uri.IsWellFormedUriString(ServiceUrl, UriKind.Absolute))
            {
                return Result.Error("Bad address", "Please, enter a valid service url.");
            }

            try
            {
                IsBusy = true;
                using (var cli = new WebApiClient(ServiceUrl))
                {
                    var res = await cli.GetAsync("api/v1/catalog/catalogtypes");
                }
                return Result.Ok("Success", "The connection to the server succeeded.");
            }
            catch (Exception ex)
            {
                return Result.Error("Error accessing remote service", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void ApplyChanges()
        {
            AppSettings.Current.DataProvider = DataProvider;
            if (IsRESTProvider)
            {
                AppSettings.Current.ServiceUrl = ServiceUrl;
            }
        }
    }
}
