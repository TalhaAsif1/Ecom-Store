using AutoMapper;
using Ecom.Dto;
using Ecom.Interfaces;
using Ecom.Models;
using Ecom.Repositoy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Ecom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IOrderInterface _orderRepository;
        private readonly IUserInterface _userRepository;
        private readonly IProductInterface _productRepository;
        private readonly IMapper _mapper;

        public OrderController(IOrderInterface orderRepository, IUserInterface userRepository, IProductInterface productRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<OrderDto>))]
        public IActionResult GetOrders()
        {
            var orders = _mapper.Map<List<OrderDto>>(_orderRepository.GetAllOrders());
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(orders);
        }

        [HttpGet("{orderId}")]
        [ProducesResponseType(200, Type = typeof(OrderDto))]
        [ProducesResponseType(400)]
        public IActionResult GetOrder(int orderId)
        {
            if (!_orderRepository.OrderExists(orderId))
                return NotFound();

            var order = _mapper.Map<OrderDto>(_orderRepository.GetOrderById(orderId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(order);
        }

        [HttpPost]
        [ProducesResponseType(201)] // Created
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(422)] // Unprocessable Entity
        [ProducesResponseType(500)] // Internal Server Error
        public IActionResult CreateOrder([FromQuery] int userId, [FromBody] OrderDto orderCreate)
        {
            if (orderCreate == null)
                return BadRequest(ModelState);

            var existingOrder = _orderRepository.GetAllOrders()
                .FirstOrDefault(o => o.OrderDate == orderCreate.OrderDate); // You may need to adjust this condition based on your requirements

            if (existingOrder != null)
            {
                ModelState.AddModelError("", "Order already exists");
                return StatusCode(422, ModelState);
            }

            if (!_userRepository.UserExists(userId))
                return NotFound("User not found");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var orderMap = _mapper.Map<Order>(orderCreate);
            orderMap.User = _userRepository.GetUserById(userId);

            if (!_orderRepository.AddOrder(orderMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }
        [HttpPut("{orderId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)] // No content
        [ProducesResponseType(404)]
        public IActionResult UpdateOrder(int orderId, [FromBody] OrderDto updateOrder)
        {
            if (updateOrder == null)
                return BadRequest(ModelState);

            if (orderId != updateOrder.Id)
                return BadRequest(ModelState);

            if (!_orderRepository.OrderExists(orderId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var orderMap = _mapper.Map<Order>(updateOrder);

            // Associate the order with the user
            var user = _userRepository.GetUserById(updateOrder.Id);
            if (user == null)
            {
                ModelState.AddModelError("", $"User with ID {updateOrder.Id} not found");
                return StatusCode(404, ModelState);
            }

            orderMap.User = user;

            if (!_orderRepository.UpdateOrder(orderMap))
            {
                ModelState.AddModelError("", "Something went wrong updating order");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{orderId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)] // No content
        [ProducesResponseType(404)]
        public IActionResult DeleteOrder(int orderId)
        {
            if (!_orderRepository.OrderExists(orderId))
                return NotFound();

            var orderToDelete = _orderRepository.GetOrderById(orderId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_orderRepository.DeleteOrder(orderToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting order");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

    }



}
