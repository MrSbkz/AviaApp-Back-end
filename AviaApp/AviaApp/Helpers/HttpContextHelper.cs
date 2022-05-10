using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace AviaApp.Helpers;

public static class HttpContextHelper
{
    public static string GetEmailFromContext(HttpContext context)
    {
        if (context.User.Identity == null) return string.Empty;
        var email = context.User.Identity.Name;
        return email ?? string.Empty;
    }

    public static IList<string> GetRolesFromContext(HttpContext context)
    {
        return context.User.Claims.Where(x => x.Type.Contains("role")).Select(x => x.Value).ToList();
    }
}