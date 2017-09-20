using System.Collections.Generic;
using eShop.Domain.Models;

namespace eShop.Providers.Contracts
{
    public interface IOrdersProvider
    {
        IList<DataPoint> GetOrdersByType(int id);
    }
}
