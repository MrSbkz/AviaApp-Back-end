using System;
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
}