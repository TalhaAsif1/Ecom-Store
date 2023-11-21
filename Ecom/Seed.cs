using Ecom.Models;
using Ecom.Data;
using System;
using System.Collections.Generic;

namespace Ecom
{
    public class Seed
    {
        private readonly DataContext dataContext;

        public Seed(DataContext context)
        {
            this.dataContext = context;
        }

        public void SeedDataContext()
        {
            if (!dataContext.Users.Any())
            {
                var seedData = new SeedData();

                dataContext.Users.AddRange(seedData.Users);
                dataContext.Orders.AddRange(seedData.Orders);
                dataContext.Products.AddRange(seedData.Products);
                dataContext.Categories.AddRange(seedData.Categories);
                dataContext.OrderDetails.AddRange(seedData.OrderDetails);

                dataContext.SaveChanges();
            }
        }
    }

    public class SeedData
    {
        public List<User> Users { get; set; }
        public List<Order> Orders { get; set; }
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }

        public SeedData()
        {
            Categories = new List<Category>
            {
                new Category { Name = "Electronics", Description = "Electronic devices" },
                new Category { Name = "Footwear", Description = "Shoes and footwear" },
                new Category { Name = "Photography", Description = "Camera and photo equipment" }
            };

            Products = new List<Product>
            {
                new Product { Name = "Laptop", Price = 1200.99m, Quantity = 50, Category = Categories[0] },
                new Product { Name = "Running Shoes", Price = 89.99m, Quantity = 100, Category = Categories[1] },
                new Product { Name = "Camera", Price = 799.99m, Quantity = 30, Category = Categories[2] }
            };

            Users = new List<User>
            {
                new User { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Password = "password123" },
                new User { FirstName = "Alice",LastName = "Johnson", Email = "alice.johnson@example.com", Password = "qwerty456" },
                new User { FirstName = "Emma", LastName = "Smith", Email = "emma.smith@example.com", Password = "pass123" }
            };

            Orders = new List<Order>
        {
            new Order { User = Users[0], OrderDate = new DateTime(2023, 1, 15), Amount = 1200.99m, OrderDetail = new List<OrderDetail>() },
            new Order { User = Users[1], OrderDate = new DateTime(2023, 2, 1), Amount = 89.99m, OrderDetail = new List<OrderDetail>() },
            new Order { User = Users[2], OrderDate = new DateTime(2023, 3, 8), Amount = 799.99m, OrderDetail = new List<OrderDetail>() }
        };

            OrderDetails = new List<OrderDetail>
        {
            new OrderDetail { Order = Orders[0], Product = Products[0], Quantity = 2, Subtotal = 2419.98m },
            new OrderDetail { Order = Orders[1], Product = Products[1], Quantity = 1, Subtotal = 89.99m },
            new OrderDetail { Order = Orders[2], Product = Products[2], Quantity = 1, Subtotal = 799.99m }
        };


        }
    }
}
