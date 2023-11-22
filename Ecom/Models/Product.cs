namespace Ecom.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity {  get; set; }

        public Category Category { get; set; }
        public ICollection<OrderDetail> OrderDetail { get; set; }   


    }
}
