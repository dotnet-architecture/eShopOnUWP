using System.Collections.Generic;

namespace eShop.Domain.Models
{
    public class Serie
    {
        public List<DataPoint> Data { get; set; }
        public int ColorIndex { get; set; }
    }
}
