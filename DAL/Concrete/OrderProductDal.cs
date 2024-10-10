using DAL.Interfaces;
using DTO;
using System.Data.SqlClient;


namespace DAL.Concrete
{
    public class OrderProductDal : IOrderProductDal
    {
        private readonly string _connectionString;

        public OrderProductDal(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<OrderProduct> GetProductsForOrder(int orderId)
        {
            List<OrderProduct> orderProducts = new List<OrderProduct>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM tblOrderProducts WHERE OrderId = @OrderId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderId", orderId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    orderProducts.Add(new OrderProduct
                    {
                        OrderId = (int)reader["OrderId"],
                        ProductId = (int)reader["ProductId"],
                        Quantity = (int)reader["Quantity"]
                    });
                }
            }

            return orderProducts;
        }
        public List<OrderProduct> GetOrderProductsByOrderId(int orderId)
        {
            List<OrderProduct> orderProducts = new List<OrderProduct>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM tblOrderProducts WHERE OrderId = @OrderId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderId", orderId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    orderProducts.Add(new OrderProduct
                    {
                        OrderId = (int)reader["OrderId"],
                        ProductId = (int)reader["ProductId"],
                        Quantity = (int)reader["Quantity"]
                    });
                }
            }

            return orderProducts;
        }
    }
}
