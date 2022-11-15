
using Common.Extensions;

using Microsoft.AspNetCore.Http;

namespace Common.Helpers
{
    public class BaseService : Service
    {
        protected int _userId;
        protected string _userName;
        public BaseService(IHttpContextAccessor httpContextAccessor)
        {
            _userId = httpContextAccessor.HttpContext.GetUserId();
            _userName = httpContextAccessor.HttpContext.GetUserName();
        }
    }

}
