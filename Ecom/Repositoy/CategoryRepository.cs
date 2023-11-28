using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Ecom.Data;
using Ecom.Interfaces;
using Ecom.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecom.Repositoy
{
    public class CategoryRepository: ICategoryRepository
    {
        private DataContext _context;
    public CategoryRepository(DataContext context)
        {
            _context = context;
        }

        public bool CategoryExists(int id)
        {
            return _context.Categories.Any(c => c.Id == id);
        }

        public bool CreateCategory(Category category)
        {
            _context.Add(category);
            return Save();
        }

        public bool DeleteCategory(Category category)
        {
            _context.Remove(category);
            return Save();
        }

        public ICollection<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }

        public Category GetCategory(int id)
        {
            return _context.Categories.Where(c => c.Id == id).FirstOrDefault();
        }



        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateCategory(Category category)
        {
            _context.Update(category);
            return Save();
        }
        public ICollection<Product> GetProductByCategory(int Id)
        {
            var category = _context.Categories
                .Include(c => c.Products) // Include the navigation property
                .FirstOrDefault(c => c.Id == Id);

            if (category != null)
            {
                return category.Products.ToList();
            }

            return new List<Product>();
        }
    }
}
