using InventorySafe.Core.Contracts.Services;
using InventorySafe.Core.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Xml.Linq;
using static ABI.System.Collections.Generic.IReadOnlyDictionary_Delegates;

namespace InventorySafe.Helpers
{
    public class SqliteHandler
    {
        public SqliteHandler()
        {
            //if (_connection == null) CreateConnection();
        }
        public static readonly Lazy<SqliteHandler> lazy = new Lazy<SqliteHandler>(() => new SqliteHandler());
        //private SqliteHandler() { }
        public SQLiteConnection _connection { get; set; }
        public static SqliteHandler Instance
        {
            get
            {
                return lazy.Value;
            }
        }
        internal SQLiteConnection CreateConnection()
        {
            //if (!File.Exists("ladyj.db"))
            //    File.WriteAllBytes("ladyj.db", Convert.FromBase64String(Inner._d));
            if (_connection == null)
            {
                string db = $"{System.IO.Directory.GetCurrentDirectory()}\\ladyj.db";
                if (!System.IO.File.Exists(db))
                {
                    MessageBox.Show("Unable to find the Database\nPlease select it now");
                    OpenFileDialog op = new OpenFileDialog();
                    op.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
                    op.Filter = ".db (*.db)|*.db";
                    op.ShowDialog();
                    if (!string.IsNullOrWhiteSpace(op.FileName))
                        db = op.FileName;
                }
                _connection = new SQLiteConnection($"Data Source={db};Version=3;Compress=True;");
                _connection.Open();
                return _connection;
            }
            return null;
        }

        internal void CloseConnection()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
        }

        public int ExecuteNonQuery(string query)
        {
            if (_connection == null) CreateConnection();
            try
            {
                SQLiteCommand command = new SQLiteCommand(query, _connection);
                return command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }

        public SQLiteDataReader ExecuteQuery(string query)
        {
            if (_connection == null) CreateConnection();
            try
            {
                using (_connection)
                {
                    SQLiteCommand command = new SQLiteCommand(query, _connection);
                    SQLiteDataReader table = command.ExecuteReader();

                    return table;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public void CreateTable(SQLiteConnection _sql)
        {
            if (_connection == null) CreateConnection();
            //CREATE TABLE \"Inventory\" (\"id\" INTEGER NOT NULL, \"Name\" VARCHAR NOT NULL,\"StockCount\" INTEGER NOT NULL,\"CostPrice\" INTEGER NOT NULL,\"SalePrice\" INTEGER NOT NULL,\"Profit\" INTEGER NOT NULL, PRIMARY KEY(\"id\" AUTOINCREMENT))
            string Createsql = "CREATE TABLE Inventory (id INTEGER NOT NULL, name VARCHAR(99) NOT NULL, stockcount INTEGER NOT NULL, costprice INTEGER NOT NULL, saleprice INTEGER NOT NULL, profit INTEGER NOT NULL, PRIMARY KEY(id AUTOINCREMENT))";
            SQLiteCommand sqlite_cmd;// = new SQLiteCommand(Createsql, _sql); 
            sqlite_cmd = _sql.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
        }
        public bool InsertData(string column, string value, string itemName)
        {
            try
            {
                //int _profit = (int.Parse(stockcount) * int.Parse(saleprice)) - (int.Parse(stockcount) * int.Parse(costprice));
                string insertQuery = $"UPDATE Inventory set {column.Trim()} = '{value}' WHERE name = '{itemName}'";
                SQLiteCommand command = new SQLiteCommand(insertQuery, SqliteHandler.Instance._connection);
                SQLiteDataReader table = command.ExecuteReader();
                Console.WriteLine(insertQuery);
                if (command.ExecuteNonQuery() == 1)
                {
                    return true;
                }
            }
            catch (Exception e) { Console.WriteLine(e); }
            return false;
        }
        public bool InsertData(Product product)
        {
            try
            {
                string name = product.Name;
                string stockcount = product.StockCount;
                string costprice = product.CostPrice;
                string saleprice = product.SalesPrice;
                //int _profit = (int.Parse(stockcount) * int.Parse(saleprice)) - (int.Parse(stockcount) * int.Parse(costprice));
                string insertQuery = $"insert into Inventory(id,name,StockCount,CostPrice,SalePrice,InitialCost,Profit,ImageBase64) values(null, '{name}','{stockcount}','{costprice}','{saleprice}','{product.InitialCost}','TBD', '{product.ImageLocation}')";
                SQLiteCommand command = new SQLiteCommand(insertQuery, SqliteHandler.Instance._connection);
                SQLiteDataReader table = command.ExecuteReader();
                Console.WriteLine(insertQuery);
                if (command.ExecuteNonQuery() == 1)
                {
                    return true;
                }
            }
            catch (Exception e) { Console.WriteLine(e); }
            return false;
        }
        public bool InsertShopData(ShopItem product)
        {
            try
            {
                string name = product.Name;
                string stockcount = product.StorePrice;
                string costprice = product.CostPrice;
                string saleprice = product.Sales;
                //int _profit = (int.Parse(stockcount) * int.Parse(saleprice)) - (int.Parse(stockcount) * int.Parse(costprice));
                string insertQuery = $"insert into Shop(id,name,storeprice,costprice,sales) values(null, '{name}','{stockcount}','{costprice}','{saleprice}')";
                SQLiteCommand command = new SQLiteCommand(insertQuery, SqliteHandler.Instance._connection);
                SQLiteDataReader table = command.ExecuteReader();
                Console.WriteLine(insertQuery);
                if (command.ExecuteNonQuery() == 1)
                {
                    return true;
                }
            }
            catch (Exception e) { Console.WriteLine(e); }
            return false;
        }
        public async Task<IEnumerable<ShopItem>> GetShopProducts()
        {
            await Task.CompletedTask;
            return ReadShopData();
        }
        public async Task<IEnumerable<Product>> GetProducts()
        {
            await Task.CompletedTask;
            return ReadData();
        }
        public void FullUpdate(Product product)
        {
            if (_connection == null) CreateConnection();
            SQLiteCommand sQLite = new SQLiteCommand(_connection);
            sQLite = _connection.CreateCommand();
            sQLite.CommandText = $"UPDATE Inventory SET StockCount = '{product.StockCount}', CostPrice = '{product.CostPrice}', SalePrice = '{product.SalesPrice}' WHERE name = '{product.Name}'";
            sQLite.ExecuteNonQuery();
            Thread.Sleep(100);
        }
        public void UpdateDB(string name, string stockcount, string profit)
        {
            if (_connection == null) CreateConnection();
            SQLiteCommand sQLite = new SQLiteCommand(_connection);
            sQLite = _connection.CreateCommand();
            string _stock = "";
            string _profit = "";
            bool exec = true;
            Thread.Sleep(100);
            sQLite.CommandText = $"UPDATE Inventory SET ";
            if (!string.IsNullOrEmpty(stockcount))
                _stock = $"StockCount = '{stockcount}'";
            if (!string.IsNullOrEmpty(profit))
                _profit = $"Profit = '{int.Parse(profit)}'";
            string final = $" WHERE name = '{name}'";
            if (string.IsNullOrEmpty(stockcount) && string.IsNullOrEmpty(profit))
                exec = false;
            else if (!string.IsNullOrEmpty(stockcount) && !string.IsNullOrEmpty(profit))
                sQLite.CommandText += $"{_stock}, {_profit}" + final;
            else if (string.IsNullOrEmpty(stockcount) && !string.IsNullOrEmpty(profit))
                sQLite.CommandText += $"{_profit} {final}";
            else if (!string.IsNullOrEmpty(stockcount) && string.IsNullOrEmpty(profit))
                sQLite.CommandText += $"{_stock} {final}";
            if (exec)
                sQLite.ExecuteNonQuery();
            Thread.Sleep(100);
        }

        public void Delete(string name)
        {
            if (_connection == null) CreateConnection();
            SQLiteCommand sqlite_cmd = new SQLiteCommand(_connection);
            sqlite_cmd = _connection.CreateCommand();
            sqlite_cmd.CommandText = $"DELETE FROM Inventory WHERE name = '{name}'";
            sqlite_cmd.ExecuteNonQuery();
            Thread.Sleep(100);
        }
        public IEnumerable<ShopItem> ReadShopData()
        {
            string query = string.Format("SELECT * FROM Shop");
            SQLiteCommand command = new SQLiteCommand(query, SqliteHandler.Instance._connection);
            SQLiteDataReader table = command.ExecuteReader();
            var productList = new List<ShopItem>();
            int count = -1;
            if (table.HasRows)
            {
                while (table.Read())
                {
                    count++;
                    string _profit = "";
                    if (!string.IsNullOrEmpty(table.GetString(6)))
                        _profit = table.GetString(6);
                    var product = new ShopItem
                    {
                        Name = table.GetString(1),
                        StorePrice = table.GetString(2),
                        CostPrice = table.GetString(3),
                        Sales = table.GetString(3),
                    };
                    productList.Add(product);
                }
            }
            return productList;
        }
        public void GetValues(string name)
        {
            string query = $"SELECT StockCount, CostPrice, SalePrice FROM Inventory WHERE Name = '{name}'";
            SQLiteCommand command = new SQLiteCommand(query, SqliteHandler.Instance._connection);
            SQLiteDataReader table = command.ExecuteReader();
            string _stock = "";
            string _cost = "";
            string _sale = "";
            if (table.HasRows)
            {
                while (table.Read())
                {
                    _stock = table.GetString(0);
                    _cost = table.GetString(1);
                    _sale = table.GetString(2);
//                    System.Diagnostics.Process.Start("notepad.exe", "Check.log");
                }
            }
            int StockCount = int.Parse(_stock);
            int CostPrice = int.Parse(_cost);
            int SalesPrice = int.Parse(_sale);
            int Profit = (StockCount * SalesPrice) - (CostPrice * StockCount);
            int Net = (Profit / 100) * 35;
            int Gross = Profit - Net;
            command.Dispose();
            string insertQ = $"UPDATE Inventory SET Profit = '{Gross}' WHERE Name = '{name}'";
            SQLiteCommand command1 = new SQLiteCommand(insertQ, SqliteHandler.Instance._connection);
            command1.ExecuteNonQuery();
        }
        public string GetImageString(string name)
        {
            string query = $"SELECT ImageBase64 FROM Inventory WHERE Name = '{name}'";
            SQLiteCommand command = new SQLiteCommand(query, SqliteHandler.Instance._connection);
            SQLiteDataReader table = command.ExecuteReader();
            if (table.HasRows)
            {
                while (table.Read())
                {
                    return table.GetString(0);
                }
            }
            return null;
        }
        public IEnumerable<Product> ReadData()
        {
            string query = string.Format("SELECT * FROM Inventory");
            SQLiteCommand command = new SQLiteCommand(query, SqliteHandler.Instance._connection);
            SQLiteDataReader table = command.ExecuteReader();
            var productList = new List<Product>();
            int count = -1;
            if (table.HasRows)
            {
                while (table.Read())
                {
                    count++;
                    string _profit = "";
                    if (!string.IsNullOrEmpty(table.GetString(7)))
                        _profit = table.GetString(7);
                    var product = new Product
                    {
                        Id = count,
                        Name = table.GetString(1),
                        StockCount = table.GetString(2),
                        CostPrice = table.GetString(3),
                        SalesPrice = table.GetString(4),
                        RestockPrice = table.GetString(5),
                        InitialCost = table.GetString(6),
                        Profit = _profit,
                    };
                    productList.Add(product);
                }
            }
            return productList;
        }
    }
}
