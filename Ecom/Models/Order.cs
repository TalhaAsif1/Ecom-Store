namespace Ecom.Models
{
    public class Order
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime OrderDate { get; set; }
        public User User { get; set; }
        public ICollection<OrderDetail> OrderDetail { get; set; }
    }
}
