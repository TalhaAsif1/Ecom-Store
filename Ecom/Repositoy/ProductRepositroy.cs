using Ecom.Data;
using Ecom.Interfaces;
using Ecom.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecom.Repositoy
{
    public class ProductRepositroy : IProductInterface
    {
        private DataContext _context;
        public ProductRepositroy(DataContext context)
        {
            _context = context;
        }
        public bool AddProduct(Product product)
        {
            _context.Add(product);
            return Save();
        }

        public bool DeleteProduct(Product product)
        {
            _context.Remove(product);
            return Save();
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _context.Products.ToList();   
        }

        public Product GetProductById(int productId)
        {
            return _context.Products.Where(p => p.Id == productId).FirstOrDefault();

    
       }

        public bool ProductExists(int id)
        {
            return _context.Products.Any(p=>p.Id== id);

        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateProduct(Product product)
        {
            _context.Update(product);

            return Save();
        }
    }
}
