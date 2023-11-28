using System.ComponentModel.DataAnnotations;

namespace Ecom.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity {  get; set; }
        public int categoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<OrderDetail> OrderDetail { get; set; }   

    }
}
