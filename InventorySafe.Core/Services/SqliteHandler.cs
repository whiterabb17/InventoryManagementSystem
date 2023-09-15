using InventorySafe.Core.Contracts.Services;
using InventorySafe.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Reflection;
using System.Text;

namespace InventorySafe.Core.Services
{
    public class SqliteHandler : ISqliteHandler
    {
        public SqliteHandler()
        {
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
        internal void CreateConnection()
        {
            //if (!File.Exists("ladyj.db"))
            //    File.WriteAllBytes("ladyj.db", Convert.FromBase64String(Inner._d));
            if (_connection == null)
            {
                string db = "D:\\Repos\\InventorySafe\\InventorySafe\\ladyj.db";
                _connection = new SQLiteConnection($"Data Source={db};Version=3;Compress=True;");
                _connection.Open();
            }
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

        public void InsertData(Product product)
        {
            string name = product.Name;
            string stockcount = product.StockCount;
            string costprice = product.CostPrice;
            string saleprice = product.SalesPrice;
            if (_connection == null) CreateConnection();
            SQLiteCommand sqlite_cmd = new SQLiteCommand(_connection);
            sqlite_cmd = _connection.CreateCommand();
            int _profit = (int.Parse(stockcount) * int.Parse(saleprice)) - (int.Parse(stockcount) * int.Parse(costprice));
            sqlite_cmd.CommandText = $"INSERT INTO Inventory (id, name, stockcount, costprice, saleprice, profit) VALUES (null, '{name}', '{stockcount}', '{costprice}', '{saleprice}', '{_profit}'); ";
            sqlite_cmd.ExecuteNonQuery();
            Thread.Sleep(100);
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            await Task.CompletedTask;
            return ReadData();
        }

        public void UpdateDB(string name, string stockcount = "", string salesprice = "")
        {
            if (_connection == null) CreateConnection();
            SQLiteCommand sQLite = new SQLiteCommand(_connection);
            sQLite = _connection.CreateCommand();
            string _stock = "";
            string _price = "";
            bool exec = true;
            sQLite.CommandText = $"UPDATE Inventory SET ";
            if (!string.IsNullOrEmpty(stockcount))
                _stock = $"stockcount = '{_stock}'";
            if (!string.IsNullOrEmpty(salesprice))
                _price = $"saleprice = '{_price}'";
            string final = $" WHERE name = '{name}'";
            if (string.IsNullOrEmpty(stockcount) && string.IsNullOrEmpty(salesprice))
                exec = false;
            else if (!string.IsNullOrEmpty(stockcount) && !string.IsNullOrEmpty(salesprice))
                sQLite.CommandText += $"{_stock}, {_price}" + final;
            else if (string.IsNullOrEmpty(stockcount) && !string.IsNullOrEmpty(salesprice))
                sQLite.CommandText += $"{_price} {final}";
            else if (!string.IsNullOrEmpty(stockcount) && string.IsNullOrEmpty(salesprice))
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

        public IEnumerable<Product> ReadData()
        {
            if (_connection == null) CreateConnection();
            var productList = new List<Product>();
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd = new SQLiteCommand(_connection);
            sqlite_cmd = _connection.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM Inventory";

            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                var product = new Product
                {
                    Id = sqlite_datareader.GetInt32(0),
                    Name = sqlite_datareader.GetString(1),
                    StockCount = sqlite_datareader.GetString(2),
                    CostPrice = sqlite_datareader.GetString(3),
                    SalesPrice = sqlite_datareader.GetString(4),
                    Profit = sqlite_datareader.GetString(5),
                };
                productList.Add(product);
            }
            return productList;
        }
    }
}
