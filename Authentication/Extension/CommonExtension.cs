using Microsoft.AspNetCore.Http;
using System.Linq;

namespace AuthenticationService.Extension
{
    public static class CommonExtension
    {
        public static string GetUserId(this HttpContext context)
        {
            if (context.User == null)
            {
                return string.Empty;
            }
            return context.User.Claims.SingleOrDefault(x => x.Type == "id").Value;
        }
    }
}
