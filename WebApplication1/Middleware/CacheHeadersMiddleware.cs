using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using System;
using System.Threading.Tasks;

namespace WebApplication1.Middleware
{
    public class CacheHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _cacheableExtensions = { ".css", ".js", ".jpg", ".jpeg", ".png", ".gif", ".ico", ".svg" };

        public CacheHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;

            if (!string.IsNullOrEmpty(path) && 
                Array.Exists(_cacheableExtensions, ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            {
                // Cache for 7 days
                var duration = TimeSpan.FromDays(7);

                context.Response.Headers.Append("Cache-Control", $"public,max-age={duration.TotalSeconds}");
                context.Response.Headers.Append("Expires", DateTime.UtcNow.Add(duration).ToString("R"));
            }

            await _next(context);
        }
    }

    public static class CacheHeadersMiddlewareExtensions
    {
        public static IApplicationBuilder UseCacheHeaders(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CacheHeadersMiddleware>();
        }
    }
}
