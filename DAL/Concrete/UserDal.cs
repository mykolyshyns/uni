using DAL.Interfaces;
using DTO;
using System.Data.SqlClient;
using System.Text;

namespace DAL.Concrete
{
    public class UserDal : IUserDal
    {
        private readonly SqlConnection _connection;

        public UserDal(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
        }

        
        public List<User> GetAll()
        {
            var users = new List<User>();
            try
            {
                using (SqlCommand command = _connection.CreateCommand())
                {
                    command.CommandText = "SELECT UserId, UserName, PasswordHash FROM tblUsers";

                    _connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            UserId = Convert.ToInt32(reader["UserId"]),
                            UserName = reader["UserName"].ToString(),
                            PasswordHash = reader["PasswordHash"].ToString()
                        });
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching users: {ex.Message}");
            }
            finally
            {
                _connection.Close();
            }

            return users;
        }

        public bool CreateUser(string username, string password)
{
    string hashedPassword = HashPassword(password);

    try
    {
        using (SqlCommand command = _connection.CreateCommand())
        {
            command.CommandText = "INSERT INTO tblUsers (UserName, PasswordHash) VALUES (@Username, @PasswordHash)";
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@PasswordHash", hashedPassword);

            _connection.Open();
            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error creating user: {ex.Message}");
        return false;
    }
    finally
    {
        _connection.Close();
    }
}

        
        public User Login(string username, string password)
        {
            string hashedPassword = HashPassword(password);

            try
            {
                _connection.Open();
                string query = "SELECT * FROM tblUsers WHERE UserName = @Username AND PasswordHash = @PasswordHash";
                SqlCommand command = new SqlCommand(query, _connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    User user = new User
                    {
                        UserId = (int)reader["UserId"],
                        UserName = (string)reader["UserName"],
                        PasswordHash = (string)reader["PasswordHash"]
                    };

                    Console.WriteLine("Login successful!");
                    return user;
                }
                else
                {
                    Console.WriteLine("Invalid credentials.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during login: {ex.Message}");
                return null;
            }
            finally
            {
                _connection.Close();
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}