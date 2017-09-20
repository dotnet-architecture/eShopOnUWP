using System;
using System.Collections.Generic;
using System.Linq;
using eShop.Domain.Models;
using eShop.Providers.Contracts;

namespace eShop.Providers
{
    public class OrdersProvider : IOrdersProvider
    {
        private static readonly List<Order> Orders = GetPreconfiguredOrders();

        public IList<DataPoint> GetOrdersByType(int id)
        {
            return Orders.Where(order => order.CatalogTypeId == id)
                .OrderBy(order => order.OrderDate)
                .Select(order => new DataPoint
                {
                    Category = $"{order.OrderDate.Day}/{order.OrderDate.Month.ToString()}",
                    Value = order.OrderTotal
                }).ToList();
        }

        private static List<Order> GetPreconfiguredOrders()
        {
            // static numbers random between 40 and 300 for emulating a data source (for example a database).
            // there are 30 numbers for each array, each value corresponds to a sales day
            var list = new List<Order>();
            list.AddRange(CreateOrdersByType(1, new[] { 150, 142, 252, 108, 299, 190, 207, 251, 208, 68, 109, 229, 188, 296, 71, 206, 141, 144, 136, 300, 98, 92,
                100, 154, 242, 130, 116, 133, 269, 185, 183, 110, 194, 48, 195, 148, 246, 164, 111, 270, 137, 228, 239, 127, 274, 189, 118, 69, 115, 43, 112, 277,
                181, 47, 78, 156, 257, 44, 147, 191 }));
            list.AddRange(CreateOrdersByType(2, new[] { 234, 68, 61, 223, 113, 65, 43, 274, 196, 269, 107, 142, 57, 290, 156, 192, 139, 186, 106, 115, 120, 59,
                258, 105, 42, 176, 90, 44, 231, 221, 256, 278, 181, 119, 162, 260, 50, 125, 93, 152, 287, 87, 70, 262, 178, 226, 134, 255, 288, 187, 84, 177, 94,
                154, 173, 123, 138, 289, 190, 128}));
            list.AddRange(CreateOrdersByType(3, new[] { 58, 153, 112, 219, 189, 231, 282, 270, 186, 201, 244, 191, 258, 91, 172, 228, 167, 253, 52, 252, 100, 140,
                76, 79, 123, 73, 92, 129, 247, 87, 289, 116, 94, 224, 194, 293, 265, 274, 114, 208, 59, 190, 41, 142, 148, 135, 86, 145, 97, 249, 170, 90, 69, 77,
                287, 187, 180, 223, 203, 192 }));
            list.AddRange(CreateOrdersByType(4, new[] { 178, 171, 65, 207, 51, 257, 241, 112, 59, 148, 120, 124, 116, 69, 170, 271, 87, 151, 74, 278, 136, 181, 43,
                281, 01, 176, 168, 299, 163, 75, 293, 218, 182, 104, 208, 53, 233, 180, 147, 144, 234, 164, 213, 249, 121, 138, 90, 276, 134, 175, 286, 186, 253,
                153, 45, 239, 222, 130, 199, 268 }));
            return list;
        }

        private static List<Order> CreateOrdersByType(int typeId, int[] values)
        {
            var index = 0;
            return values.Select(value => new Order
            {
                OrderTotal = values[index],
                OrderDate = DateTime.Today.AddDays(-(index++)),
                CatalogTypeId = typeId
            }).ToList();
        }
    }
}
