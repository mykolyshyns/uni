using DAL.Interfaces;
using DTO;
using System.Data.SqlClient;

namespace DAL.Concrete
{
    public class OrderDal : IOrderDal
    {
        private readonly string _connectionString;

        public OrderDal(string connectionString)
        {
            _connectionString = connectionString;
        }
        public List<Order> GetAllOrdersForUser(int userId)
        {
            var orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM tblOrders WHERE UserId = @UserId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        orders.Add(new Order
                        {
                            OrderId = (int)reader["OrderId"],
                            UserId = (int)reader["UserId"],
                            TotalPrice = (decimal)reader["TotalPrice"],
                            OrderDate = (DateTime)reader["OrderDate"]
                        });
                    }
                }
            }

            return orders;
        }

        public Order GetOrderById(int orderId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM tblOrders WHERE OrderId = @OrderId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderId", orderId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new Order
                    {
                        OrderId = (int)reader["OrderId"],
                        UserId = (int)reader["UserId"],
                        TotalPrice = (decimal)reader["TotalPrice"],
                        OrderDate = (DateTime)reader["OrderDate"]
                    };
                }
                return null;
            }
        }

        public Order RepeatOrder(int orderId, int userId)
        {
            Order originalOrder = GetOrderById(orderId);
            if (originalOrder == null || originalOrder.UserId != userId)
            {
                return null;
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO tblOrders (UserId, TotalPrice, OrderDate) OUTPUT INSERTED.OrderId VALUES (@UserId, @TotalPrice, @OrderDate)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@TotalPrice", originalOrder.TotalPrice);
                    command.Parameters.AddWithValue("@OrderDate", DateTime.Now);

                    int newOrderId = (int)command.ExecuteScalar();

                    List<OrderProduct> orderProducts = GetOrderProductsByOrderId(orderId);
                    foreach (var op in orderProducts)
                    {
                        using (SqlCommand commandProduct = new SqlCommand("INSERT INTO tblOrderProducts (OrderId, ProductId, Quantity) VALUES (@OrderId, @ProductId, @Quantity)", connection))
                        {
                            commandProduct.Parameters.AddWithValue("@OrderId", newOrderId);
                            commandProduct.Parameters.AddWithValue("@ProductId", op.ProductId);
                            commandProduct.Parameters.AddWithValue("@Quantity", op.Quantity);
                            commandProduct.ExecuteNonQuery();
                        }
                    }

                    return new Order { OrderId = newOrderId, UserId = userId, TotalPrice = originalOrder.TotalPrice, OrderDate = DateTime.Now };
                }
            }
        }
        public void UpdateOrderTotalPrice(int orderId, decimal totalPrice)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE tblOrders SET TotalPrice = @TotalPrice WHERE OrderId = @OrderId";
                    command.Parameters.AddWithValue("@TotalPrice", totalPrice);
                    command.Parameters.AddWithValue("@OrderId", orderId);

                    command.ExecuteNonQuery();
                }
            }
        }


        public List<OrderProduct> GetOrderProductsByOrderId(int orderId)
        {
            List<OrderProduct> orderProducts = new List<OrderProduct>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT * FROM tblOrderProducts WHERE OrderId = @OrderId", connection))
                {
                    command.Parameters.AddWithValue("@OrderId", orderId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var orderProduct = new OrderProduct
                            {
                                OrderId = (int)reader["OrderId"],
                                ProductId = (int)reader["ProductId"],
                                Quantity = (int)reader["Quantity"]
                            };
                            orderProducts.Add(orderProduct);
                        }
                    }
                }
            }
            return orderProducts;
        }


        public void ShowOrderHistory(int userId)
        {
            List<Order> orders = GetAllOrdersForUser(userId);

            foreach (var order in orders)
            {
                Console.WriteLine($"Order ID: {order.OrderId}, Total Price: {order.TotalPrice}, Date: {order.OrderDate}");
            }
        }

        public List<Order> SortOrders(List<Order> orders, string sortBy, bool ascending)
        {
            switch (sortBy.ToLower())
            {
                case "date":
                    return ascending
                        ? orders.OrderBy(o => o.OrderDate).ToList()
                        : orders.OrderByDescending(o => o.OrderDate).ToList();
                case "totalprice":
                    return ascending
                        ? orders.OrderBy(o => o.TotalPrice).ToList()
                        : orders.OrderByDescending(o => o.TotalPrice).ToList();
                default:
                    throw new ArgumentException("Invalid sort criteria");
            }
        }
        public void AddOrder(Order order, List<OrderProduct> orderProducts)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO tblOrders (UserId, TotalPrice, OrderDate) OUTPUT INSERTED.OrderId VALUES (@UserId, @TotalPrice, @OrderDate)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", order.UserId);
                    command.Parameters.AddWithValue("@TotalPrice", order.TotalPrice);
                    command.Parameters.AddWithValue("@OrderDate", order.OrderDate);

                    int newOrderId = (int)command.ExecuteScalar();

                    foreach (var orderProduct in orderProducts)
                    {
                        using (SqlCommand commandProduct = new SqlCommand("INSERT INTO tblOrderProducts (OrderId, ProductId, Quantity) VALUES (@OrderId, @ProductId, @Quantity)", connection))
                        {
                            commandProduct.Parameters.AddWithValue("@OrderId", newOrderId);
                            commandProduct.Parameters.AddWithValue("@ProductId", orderProduct.ProductId);
                            commandProduct.Parameters.AddWithValue("@Quantity", orderProduct.Quantity);
                            commandProduct.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public void DeleteOrder(int orderId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string deleteOrderProductsQuery = "DELETE FROM tblOrderProducts WHERE OrderId = @OrderId";
                using (SqlCommand command = new SqlCommand(deleteOrderProductsQuery, connection))
                {
                    command.Parameters.AddWithValue("@OrderId", orderId);
                    command.ExecuteNonQuery();
                }

                string deleteOrderQuery = "DELETE FROM tblOrders WHERE OrderId = @OrderId";
                using (SqlCommand command = new SqlCommand(deleteOrderQuery, connection))
                {
                    command.Parameters.AddWithValue("@OrderId", orderId);
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}
