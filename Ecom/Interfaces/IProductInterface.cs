using Ecom.Models;

namespace Ecom.Interfaces
{
    public interface IProductInterface
    {
        Product GetProductById(int productId);
        IEnumerable<Product> GetAllProducts();
        bool AddProduct(Product product, int categoryId);
        bool UpdateProduct(Product product);
        bool DeleteProduct(Product product);
        bool Save();
        bool ProductExists(int id);
        bool ProductNameExists(string productName, int categoryId);
    }
}
