using DAL.Interfaces;
using DTO;
using System.Data.SqlClient;

namespace DAL.Concrete
{
    public class ProductDal : IProductDal
    {
        private readonly string _connectionString;

        public ProductDal(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Product> GetAllProducts()
        {
            List<Product> products = new List<Product>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM tblProducts";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        ProductId = (int)reader["ProductId"],
                        ProductName = reader["ProductName"].ToString(),
                        Price = (decimal)reader["Price"]
                    });
                }
            }

            return products;
        }
        public List<Product> SearchProducts(string productName)
        {
            List<Product> products = new List<Product>();

            string query = "SELECT * FROM tblProducts WHERE ProductName LIKE @ProductName";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductName", "%" + productName + "%");

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            Product product = new Product
                            {
                                ProductId = (int)reader["ProductId"],
                                ProductName = (string)reader["ProductName"],
                                Price = (decimal)reader["Price"],

                            };
                            products.Add(product);
                        }
                    }
                }
            }

            return products;
        }
        public List<Product> SortProducts(List<Product> products, string sortBy, bool ascending)
        {
            switch (sortBy.ToLower())
            {
                case "name":
                    return ascending
                    ? products.OrderBy(p => p.ProductName).ToList()
                    : products.OrderByDescending(p => p.ProductName).ToList();

                case "price":
                    return ascending
                        ? products.OrderBy(o => o.Price).ToList()
                        : products.OrderByDescending(o => o.Price).ToList();
                default:
                    throw new ArgumentException("Invalid sort criteria");
            }
        }
        
        public void DeleteProduct(int productId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string deleteProductQuery = "DELETE FROM tblProducts WHERE ProductId = @ProductId";
                using (SqlCommand command = new SqlCommand(deleteProductQuery, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);
                    command.ExecuteNonQuery();
                }
            }
        }
        
        
        public Product AddProduct(Product product)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO tblProducts (ProductName, Price) OUTPUT INSERTED.ProductId VALUES (@ProductName, @Price)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductName", product.ProductName);
                    command.Parameters.AddWithValue("@Price", product.Price);

                    int newProductId = (int)command.ExecuteScalar();

                    return new Product
                    {
                        ProductId = newProductId,
                        ProductName = product.ProductName,
                        Price = product.Price
                    };
                }
            }
        }
        
        public Product GetProductById(int productId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM tblProducts WHERE ProductId = @ProductId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductId", productId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new Product
                    {
                        ProductId = (int)reader["ProductId"],
                        ProductName = reader["ProductName"].ToString(),
                        Price = (decimal)reader["Price"]
                    };
                }
                return null;
            }
        }
    }
}
