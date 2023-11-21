using Ecom.Data;
using Ecom.Interfaces;
using Ecom.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Ecom.Repositoy
{
    public class OrderRepository : IOrderInterface
    {
        private readonly DataContext _context;
        public OrderRepository(DataContext context)
        {
            _context = context;
        }
        public bool AddOrder(Order order)
        {
            _context.Add(order);
            return Save();
        }

        public bool DeleteOrder(Order orderId)
        {
            _context.Remove(orderId);
            return Save();
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return _context.Orders.ToList();
        }

        public Order GetOrderById(int orderId)
        {
            return _context.Orders.Where(o=>o.Id==orderId).FirstOrDefault();

        }

        public bool OrderExists(int id)
        {
            return _context.Orders.Any(o => o.Id == id);        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateOrder(Order order)
        {
             _context.Update(order);
            return Save();
           
        }
    }
}
