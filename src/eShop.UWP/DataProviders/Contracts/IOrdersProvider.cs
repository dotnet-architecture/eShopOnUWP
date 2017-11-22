using System;
using System.Collections.Generic;

namespace eShop.Providers
{
    public interface IOrdersProvider
    {
        IList<DataPoint> GetOrdersByType(int id);
    }

    public class DataPoint
    {
        public double Value { get; set; }
        public string Category { get; set; }
    }
}
