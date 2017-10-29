using System;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Data.SqlClient;

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

        public DataProviderType DataProvider
        {
            get
            {
                if (IsRESTProvider) return DataProviderType.REST;
                if (IsSqlProvider) return DataProviderType.Sql;
                return DataProviderType.Local;
            }
        }

        public bool HasChanges => DataProvider != AppSettings.Current.DataProvider || ServiceUrl != AppSettings.Current.ServiceUrl || SqlConnectionString != AppSettings.Current.SqlConnectionString;

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

        private bool _isSqlProvider;
        public bool IsSqlProvider
        {
            get { return _isSqlProvider; }
            set { Set(ref _isSqlProvider, value); }
        }

        private string _serviceUrl;
        public string ServiceUrl
        {
            get { return _serviceUrl; }
            set { Set(ref _serviceUrl, value); }
        }

        private string _sqlConnectionString;
        public string SqlConnectionString
        {
            get { return _sqlConnectionString; }
            set { Set(ref _sqlConnectionString, value); }
        }

        public Visibility CancelVisibility => NavigationService.CanGoBack ? Visibility.Visible : Visibility.Collapsed;

        private bool _isRESTBusy = false;
        public bool IsRESTBusy
        {
            get { return _isRESTBusy; }
            set { Set(ref _isRESTBusy, value); }
        }

        private bool _isSqlBusy = false;
        public bool IsSqlBusy
        {
            get { return _isSqlBusy; }
            set { Set(ref _isSqlBusy, value); }
        }

        public ICommand ValidateRESTConnectionCommand => new RelayCommand(OnValidateRESTConnection);
        private async void OnValidateRESTConnection()
        {
            var result = await ValidateAsync();
            await DialogBox.ShowAsync(result);
        }

        public ICommand ValidateSqlConnectionCommand => new RelayCommand(OnValidateSqlConnection);
        private async void OnValidateSqlConnection()
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
            IsSqlProvider = AppSettings.Current.DataProvider == DataProviderType.Sql;
            ServiceUrl = AppSettings.Current.ServiceUrl;
            SqlConnectionString = AppSettings.Current.SqlConnectionString;
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
                return await ValidateRESTConnectionAsync();
            }
            if (IsSqlProvider)
            {
                return await ValidateSqlConnectionAsync();
            }
            return Result.Ok();
        }

        private async Task<Result> ValidateRESTConnectionAsync()
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
                IsRESTBusy = true;
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
                IsRESTBusy = false;
            }
        }

        private async Task<Result> ValidateSqlConnectionAsync()
        {
            if (String.IsNullOrEmpty(SqlConnectionString))
            {
                return Result.Error("Empty connection string", "Please, enter a valid connection string.");
            }

            try
            {
                var cnn = new SqlConnection(SqlConnectionString);
            }
            catch
            {
                return Result.Error("Bad connection string", "Please, enter a valid connection string.");
            }

            try
            {
                IsSqlBusy = true;

                using (var cnn = new SqlConnection(SqlConnectionString))
                {
                    await cnn.OpenAsync();
                }

                return Result.Ok("Success", "The connection to the Sql Server succeeded.");
            }
            catch (Exception ex)
            {
                return Result.Error("Error connecting to Sql Server", ex.Message);
            }
            finally
            {
                IsSqlBusy = false;
            }
        }

        public void ApplyChanges()
        {
            AppSettings.Current.DataProvider = DataProvider;
            if (IsRESTProvider)
            {
                AppSettings.Current.ServiceUrl = ServiceUrl;
            }
            if (IsSqlProvider)
            {
                AppSettings.Current.SqlConnectionString = SqlConnectionString;
            }
        }
    }
}
