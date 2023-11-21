using Ecom.Models;

namespace Ecom.Interfaces
{
    public interface IOrderInterface
    {
        Order GetOrderById(int orderId);
        IEnumerable<Order> GetAllOrders();
        bool AddOrder(Order order);
        bool UpdateOrder(Order order);
        bool DeleteOrder(Order orderId);
        bool Save();
        bool OrderExists(int id);

    }
}
