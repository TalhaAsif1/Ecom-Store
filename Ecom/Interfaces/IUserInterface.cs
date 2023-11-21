using Ecom.Models;

namespace Ecom.Interfaces
{
    public interface IUserInterface
    {
        User GetUserById(int userId);
        IEnumerable<User> GetAllUsers();
        bool AddUser(User user);
        bool UpdateUser(User user);
        bool DeleteUser(User user);
        bool Save();
        bool UserExists(int id);
    }
}
