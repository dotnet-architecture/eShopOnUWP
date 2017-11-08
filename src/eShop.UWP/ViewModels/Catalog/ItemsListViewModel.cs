using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using Telerik.UI.Xaml.Controls.Grid;

using eShop.UWP.Models;

namespace eShop.UWP.ViewModels
{
    public enum ListCommandBarMode
    {
        Idle,
        ItemsSelected,
        AllSelected
    }

    public class ItemsListViewModel : ViewModelBase
    {
        public ItemsListViewModel()
        {
        }

        public RadDataGrid ItemsControl { get; set; }

        public bool IsActive { get; set; }

        public ListCommandBarMode Mode { get; set; }

        private ObservableCollection<CatalogItemModel> _items = null;
        public ObservableCollection<CatalogItemModel> Items
        {
            get { return _items; }
            set { Set(ref _items, value); }
        }

        private bool _isCommandBarOpen = false;
        public bool IsCommandBarOpen
        {
            get { return _isCommandBarOpen; }
            set { Set(ref _isCommandBarOpen, value); }
        }

        public bool IsSelectAllVisible => Mode != ListCommandBarMode.AllSelected;
        public bool IsClearVisible => Mode == ListCommandBarMode.AllSelected;
        public bool IsCancelVisible => Mode != ListCommandBarMode.Idle;
        public bool IsSeparatorVisible => IsDeleteVisible;
        public bool IsDeleteVisible => Mode == ListCommandBarMode.ItemsSelected || Mode == ListCommandBarMode.AllSelected;

        public ICommand SelectionChangedCommand => new RelayCommand<DataGridSelectionChangedEventArgs>(OnSelectionChanged);

        public ICommand SelectAllCommand => new RelayCommand(OnSelectAll);
        public ICommand ClearCommand => new RelayCommand(OnClear);
        public ICommand CancelCommand => new RelayCommand(OnCancel);
        public ICommand DeleteCommand => new RelayCommand(OnDelete);

        public void UpdateExternalSelection()
        {
            foreach (var item in Items.Where(r => r.IsSelected))
            {
                ItemsControl.SelectItem(item);
            }
            foreach (var item in Items.Where(r => !r.IsSelected))
            {
                ItemsControl.DeselectItem(item);
            }
        }

        private bool _cancelOnSelectionChanged = false;

        private void OnSelectionChanged(DataGridSelectionChangedEventArgs args)
        {
            if (_cancelOnSelectionChanged)
            {
                return;
            }

            ApplySelection(args.AddedItems, true);
            ApplySelection(args.RemovedItems, false);

            int count = Items.Count(r => r.IsSelected);
            if (count == 0)
            {
                IsCommandBarOpen = false;
                Mode = ListCommandBarMode.Idle;
            }
            else if (count < Items.Count)
            {
                IsCommandBarOpen = true;
                Mode = ListCommandBarMode.ItemsSelected;
            }
            else
            {
                IsCommandBarOpen = true;
                Mode = ListCommandBarMode.AllSelected;
            }

            UpdateCommandBar();
        }

        private void OnSelectAll()
        {
            ApplySelection(Items, true);
            ItemsControl.SelectAll();
            Mode = ListCommandBarMode.AllSelected;
            IsCommandBarOpen = true;
            UpdateCommandBar();
        }

        private void OnClear()
        {
            ApplySelection(Items, false);
            ItemsControl.DeselectAll();
            Mode = ListCommandBarMode.Idle;
            IsCommandBarOpen = true;
            UpdateCommandBar();
        }

        private void OnCancel()
        {
            ApplySelection(Items, false);
            ItemsControl.DeselectAll();
            Mode = ListCommandBarMode.Idle;
            IsCommandBarOpen = false;
            UpdateCommandBar();
        }

        private async void OnDelete()
        {
            //ShellViewModel.Current.EnableView(false);
            if (await DialogBox.ShowAsync("Confirm Delete", "Are you sure you want to delete selected items?", "Ok", "Cancel"))
            {
                _cancelOnSelectionChanged = true;
                foreach (var item in Items.Where(r => r.IsSelected).ToArray())
                {
                    Items.Remove(item);
                }
                _cancelOnSelectionChanged = false;
            }
            //ShellViewModel.Current.EnableView(true);

            IsCommandBarOpen = false;
            UpdateCommandBar();
        }

        public void UpdateCommandBar()
        {
            RaisePropertyChanged("IsSelectAllVisible");
            RaisePropertyChanged("IsClearVisible");
            RaisePropertyChanged("IsCancelVisible");
            RaisePropertyChanged("IsSeparatorVisible");
            RaisePropertyChanged("IsDeleteVisible");
        }

        private void ApplySelection(IEnumerable<object> items, bool isSelected)
        {
            foreach (CatalogItemModel item in items)
            {
                item.IsSelected = isSelected;
            }
        }
    }
}
