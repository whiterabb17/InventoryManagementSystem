using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySafe.Core.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string StockCount { get; set; }
        public string CostPrice { get; set; }
        public string SalesPrice { get; set; }
        public string Profit { get; set; }

    }
}
