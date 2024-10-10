using DAL.Interfaces;
using DTO;
using System.Data.SqlClient;

namespace DAL.Concrete
{
    public class CommentDal : ICommentDal
    {
        private readonly string _connectionString;

        public CommentDal(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Comment> GetCommentsByProductId(int productId)
        {
            List<Comment> comments = new List<Comment>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM tblComments WHERE ProductId = @ProductId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductId", productId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    comments.Add(new Comment
                    {
                        CommentId = (int)reader["CommentId"],
                        CommentText = reader["CommentText"].ToString(),
                        CommentTime = (DateTime)reader["CommentTime"],
                        ProductId = (int)reader["ProductId"],
                        UserId = (int)reader["UserId"]
                    });
                }
            }

            return comments;
        }

        public void AddComment(Comment comment)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO tblComments (ProductId, UserId, CommentText, CommentTime) VALUES (@ProductId, @UserId, @Comment" +
                    "Text, @CommentTime)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductId", comment.ProductId);
                command.Parameters.AddWithValue("@UserId", comment.UserId);
                command.Parameters.AddWithValue("@CommentText", comment.CommentText);
                command.Parameters.AddWithValue("@CommentTime", DateTime.Now);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
