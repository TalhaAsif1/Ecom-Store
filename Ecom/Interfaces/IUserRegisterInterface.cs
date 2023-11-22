using Ecom.Dto;
using Ecom.Models;
using System.Threading.Tasks;

namespace Ecom.Interfaces
{
    public interface IUserRegisterInterface
    {
        string GetMyName();
        Task<UserRegister> RegisterUser(UserRegisterDto request);
        Task<UserRegister> GetUserByUsername(string username);
    }
}
