using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace InventorySafe.Helpers
{
    public class InnerConstants
    {
        public static ObservableCollection<Product> Source { get; } = new ObservableCollection<Product>();
        public static ObservableCollection<ShopItem> ShopSource { get; } = new ObservableCollection<ShopItem>();
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string StockCount { get; set; }
        public string CostPrice { get; set; }
        public string SalesPrice { get; set; }
        public string RestockPrice { get; set; }
        public string InitialCost { get; set; }
        //public string EmployeePrice => Convert.ToString(int.Parse(SalesPrice) / 2);
        public string Profit { get; set; }
        public string ImageLocation { get; set; }

    }

    public class ShopItem
    { 
        public string Name { get; set; }
        public string StorePrice { get; set; }
        public string CostPrice { get; set; }
        public string Sales { get; set; }
    }

    public class Grossing
    { 
        public string TotalSales { get; set; }
        public string AdminCut { get; set; }
        public string AdminPerc = "35%";
        public string Split { get; set; }
        public string Jordan { get; set; }
        public string Josh { get; set; }
        public string Scott { get; set; }
    }
}
