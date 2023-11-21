using Ecom.Models;

namespace Ecom.Dto
{
    public class OrderDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
