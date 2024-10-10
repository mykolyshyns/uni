using NUnit.Framework;
using DAL.Concrete;
using DTO;
using System.Collections.Generic;

namespace DAL.Tests
{
    [TestFixture]
    public class OrderDalTests
    {
        private OrderDal _orderDal;
        private string _testConnectionString;

        [SetUp]
        public void Setup()
        {
            _testConnectionString = "Data Source=DESKTOP-SKM2D9K;Initial Catalog=TestsBackup;Integrated Security=True;TrustServerCertificate=True";
            _orderDal = new OrderDal(_testConnectionString);
        }

        [Test]
        public void GetAllOrdersForUser_ReturnsOrdersForUser()
        {
            int userId = 1;

            List<Order> orders = _orderDal.GetAllOrdersForUser(userId);

            Assert.IsNotNull(orders);
            Assert.IsNotEmpty(orders);
            Assert.AreEqual(userId, orders[0].UserId);
        }

        [Test]
        public void GetOrderById_ReturnsCorrectOrder()
        {
            int orderId = 1;

            Order order = _orderDal.GetOrderById(orderId);

            Assert.IsNotNull(order);
            Assert.AreEqual(orderId, order.OrderId);
        }

        [Test]
        public void RepeatOrder_CreatesNewOrderWithSameProducts()
        {
            int originalOrderId = 1;
            int userId = 1;

            Order newOrder = _orderDal.RepeatOrder(originalOrderId, userId);

            Assert.IsNotNull(newOrder);
            Assert.AreNotEqual(originalOrderId, newOrder.OrderId);
            Assert.AreEqual(userId, newOrder.UserId);
        }

        [Test]
        public void AddOrder_AddsOrderAndOrderProducts()
        {
            Order newOrder = new Order { UserId = 1, TotalPrice = 100, OrderDate = System.DateTime.Now };
            List<OrderProduct> orderProducts = new List<OrderProduct>
            {
                new OrderProduct { ProductId = 1, Quantity = 2 },
                new OrderProduct { ProductId = 2, Quantity = 3 }
            };

            _orderDal.AddOrder(newOrder, orderProducts);

            Order savedOrder = _orderDal.GetOrderById(newOrder.OrderId);
            Assert.IsNotNull(savedOrder);
            Assert.AreEqual(newOrder.TotalPrice, savedOrder.TotalPrice);
        }

        [Test]
        public void DeleteOrder_RemovesOrderAndOrderProducts()
        {
            int orderIdToDelete = 2;

            _orderDal.DeleteOrder(orderIdToDelete);

            Order deletedOrder = _orderDal.GetOrderById(orderIdToDelete);
            Assert.IsNull(deletedOrder);
        }

        [Test]
        public void SortOrders_ShouldSortOrdersByDateAscending()
        {
            var orders = new List<Order>
            {
                new Order { OrderId = 1, OrderDate = DateTime.Now.AddDays(-2) },
                new Order { OrderId = 2, OrderDate = DateTime.Now.AddDays(-1) }
            };

            var sortedOrders = _orderDal.SortOrders(orders, "date", true);

            Assert.AreEqual(1, sortedOrders[0].OrderId);
        }
    }
}