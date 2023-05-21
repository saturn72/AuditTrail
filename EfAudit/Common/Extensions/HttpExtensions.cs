using System.Security.Claims;

namespace Microsoft.AspNetCore.Http
{
    public static class HttpExtensions
    {
        public static string? GetSubjectId(this IHttpContextAccessor accessor)
        {
            if (accessor == default)
                return default;
            return accessor?.HttpContext?.GetSubjectId();

        }
        public static string? GetSubjectId(this HttpContext httpContext)
        {
            if (httpContext == default)
                return default;

            return httpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}