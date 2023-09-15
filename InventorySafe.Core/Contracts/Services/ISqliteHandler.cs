using InventorySafe.Core.Models;

namespace InventorySafe.Core.Contracts.Services;

public interface ISqliteHandler
{
    Task<IEnumerable<Product>> GetProducts();
    void UpdateDB(string name, string stockcount = "", string salesprice = "");
    void InsertData(Product product);
    void Delete(string name);
}
