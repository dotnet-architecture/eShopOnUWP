using System;

using GalaSoft.MvvmLight;

namespace eShop.UWP.ViewModels
{
    public class StatisticsViewModel : CommonViewModel
    {
        public StatisticsViewModel()
        {
            HeaderText = "Statistics";
        }

        private string _text = "Statistics";
        public string Text
        {
            get { return _text; }
            set { Set(ref _text, value); }
        }
    }
}
