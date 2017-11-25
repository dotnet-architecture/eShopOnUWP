using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using eShop.UWP.Models;
using eShop.Providers;

namespace eShop.UWP.ViewModels
{
    public class StatisticsViewModel : CommonViewModel
    {
        private const int NumberOfValuesForLastOneDay = 1;
        private const int NumberOfValuesForLastWeek = 7;
        private const int NumberOfValuesForLastMonth = 30;
        private const int InterpolationValue = 7;

        private readonly ICatalogProvider _catalogProvider;
        private readonly IOrdersProvider _ordersProvider;
        private readonly Dictionary<int, bool> _filterValues = new Dictionary<int, bool>();
        private readonly List<int> _totalOrders = new List<int> { 15, 30, 55, 73, 80 };

        private bool _firstFilter = true;
        private bool _secondFilter = true;
        private bool _thirdFilter = true;
        private bool _fourthFilter = true;
        private bool _isCheckedFirstTime;
        private bool _isCheckedSecondTime;
        private bool _isCheckedThirdTime;

        private double _totalSales;
        private List<CatalogTypeModel> _catalogTypes;
        private ObservableCollection<List<DataPoint>> _series;
        private List<int> _filteredTotalOrders;

        public StatisticsViewModel(ICatalogProvider catalogProvider, IOrdersProvider ordersProvider)
        {
            _catalogProvider = catalogProvider;
            _ordersProvider = ordersProvider;
        }

        public bool FirstFilter
        {
            get => _firstFilter;
            set
            {
                Set(ref _firstFilter, value);
                ChangeFilter(0, value);
            }
        }

        public bool SecondFilter
        {
            get => _secondFilter;
            set
            {
                Set(ref _secondFilter, value);
                ChangeFilter(1, value);
            }
        }

        public bool ThirdFilter
        {
            get => _thirdFilter;
            set
            {
                Set(ref _thirdFilter, value);
                ChangeFilter(2, value);
            }
        }

        public bool FourthFilter
        {
            get => _fourthFilter;
            set
            {
                Set(ref _fourthFilter, value);
                ChangeFilter(3, value);
            }
        }

        public bool IsCheckedFirstTime
        {
            get => _isCheckedFirstTime;
            set
            {
                Set(ref _isCheckedFirstTime, value);

                _isCheckedSecondTime = false;
                _isCheckedThirdTime = false;
                RaisePropertyChanged(() => IsCheckedSecondTime);
                RaisePropertyChanged(() => IsCheckedThirdTime);

                LoadSeries();
            }
        }

        public bool IsCheckedSecondTime
        {
            get => _isCheckedSecondTime;
            set
            {
                Set(ref _isCheckedSecondTime, value);

                _isCheckedFirstTime = false;
                _isCheckedThirdTime = false;
                RaisePropertyChanged(() => IsCheckedFirstTime);
                RaisePropertyChanged(() => IsCheckedThirdTime);

                LoadSeries();
            }
        }

        public bool IsCheckedThirdTime
        {
            get => _isCheckedThirdTime;
            set
            {
                Set(ref _isCheckedThirdTime, value);

                _isCheckedFirstTime = false;
                _isCheckedSecondTime = false;
                RaisePropertyChanged(() => IsCheckedFirstTime);
                RaisePropertyChanged(() => IsCheckedSecondTime);

                LoadSeries();
            }
        }

        public double TotalSales
        {
            get => _totalSales;
            set => Set(ref _totalSales, value);
        }

        public ObservableCollection<List<DataPoint>> Series
        {
            get => _series;
            set => Set(ref _series, value);
        }

        public List<int> FilteredTotalOrders
        {
            get => _filteredTotalOrders;
            set => Set(ref _filteredTotalOrders, value);
        }

        public async void Load()
        {
            if (_catalogTypes == null)
            {
                await LoadCatalogTypesAsync();
            }
            LoadSeries();
        }

        private void ChangeFilter(int index, bool value)
        {
            _filterValues[_catalogTypes[index].Id] = value;
            LoadSeries();
        }

        private async Task LoadCatalogTypesAsync()
        {
            var types = await _catalogProvider.GetCatalogTypesAsync();
            _catalogTypes = types.ToList();

            foreach (var type in _catalogTypes)
            {
                _filterValues.Add(type.Id, true);
            }
        }

        private void LoadSeries()
        {
            var series = new ObservableCollection<List<DataPoint>>();
            var offset = GetOffset();
            double totalSales = 0;

            foreach (var type in _catalogTypes)
            {
                var serie = _filterValues[type.Id] ? LoadDataTypes(type.Id) : new List<DataPoint>();
                if (serie.Count > 0)
                {
                    serie = offset != 0 ? serie.GetRange(0, offset) : serie;
                    totalSales += serie.Sum(item => item.Value);
                    serie = InterpolateListValues(serie, InterpolationValue);
                }
                series.Add(serie);
            }

            Series = series;
            TotalSales = totalSales;

            LoadTotalOrders();
        }

        private List<DataPoint> InterpolateListValues(List<DataPoint> listToInterpolate, int interpolationValue)
        {
            if (InterpolationValue > listToInterpolate.Count)
            {
                return listToInterpolate;
            }

            var interpolationResult = listToInterpolate.Count / interpolationValue;

            var interpolatedList = new List<DataPoint>();
            for (var i = 0; i + interpolationResult < listToInterpolate.Count; i = i + interpolationResult)
            {
                interpolatedList.Add(new DataPoint { Category = listToInterpolate[i].Category, Value = listToInterpolate[i].Value + listToInterpolate[i + interpolationResult].Value / interpolationResult });
            }

            return interpolatedList;
        }

        private List<DataPoint> LoadDataTypes(int typeId)
        {
            var data = _ordersProvider.GetOrdersByType(typeId);
            return data.ToList();
        }

        private void LoadTotalOrders()
        {
            var offset = GetTotalOffset();

            var list = _catalogTypes.Take(4).Select((t, i) => _filterValues[t.Id] ? _totalOrders[i] + offset : 0).ToList();

            FilteredTotalOrders = list;
        }

        private int GetOffset()
        {
            var offset = 0;
            offset += _isCheckedFirstTime ? NumberOfValuesForLastOneDay : 0;
            offset += _isCheckedSecondTime ? NumberOfValuesForLastWeek : 0;
            offset += _isCheckedThirdTime ? NumberOfValuesForLastMonth : 0;
            return offset;
        }

        private int GetTotalOffset()
        {
            var offset = 0;
            offset += _isCheckedFirstTime ? 3 : 0;
            offset += _isCheckedSecondTime ? 10 : 0;
            offset += _isCheckedThirdTime ? 15 : 0;
            return offset;
        }
    }
}
