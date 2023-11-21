namespace Ecom.Models
{
    public class OrderDetail
    {
        public Order Order { get; set; }
        public Product Product { get; set; }
        public int OrderDetailId {  get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }
}
