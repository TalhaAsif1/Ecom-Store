using Ecom.Interfaces;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace Ecom.Repositoy
{
    public class UserRegisterRepository : IUserRegisterInterface
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRegisterRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string GetMyName()
        {
            var result = string.Empty;
            if(_httpContextAccessor.HttpContext !=null)
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            }
            return result;
        }
    }
}
