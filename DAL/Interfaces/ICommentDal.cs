using DTO;

namespace DAL.Interfaces
{
    public interface ICommentDal
    {
        List<Comment> GetCommentsByProductId(int productId);
        void AddComment(Comment comment);
    }
}
