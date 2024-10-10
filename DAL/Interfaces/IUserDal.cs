using DTO;

namespace DAL.Interfaces
{
    public interface IUserDal
    {
        List<User> GetAll();
        User Login(string username, string password);
        bool CreateUser(string username, string password);
    }
}
