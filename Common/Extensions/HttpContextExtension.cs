using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
using System.Security.Claims;


namespace Common.Extensions
{
    public static class HttpContextExtension
    {
        public static bool IsLogin(this HttpContext httpContext)
        {
            return httpContext.User.Identity.IsAuthenticated;
        }

        public static int GetUserId(this HttpContext httpContext)
        {
            var currentUserId = httpContext.User?.FindFirst(x => x.Type == "UserId")?.Value;
            int.TryParse(currentUserId, out int userId);
            return userId;
        }

        public static string GetUserName(this HttpContext httpContext)
        {
            return httpContext.User?.FindFirst(x => x.Type == ClaimTypes.Name)?.Value;
        }
    }
}
