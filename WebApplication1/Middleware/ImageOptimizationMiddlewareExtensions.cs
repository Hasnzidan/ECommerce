using Microsoft.AspNetCore.Builder;

namespace WebApplication1.Middleware
{
    public static class ImageOptimizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseImageOptimization(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ImageOptimizationMiddleware>();
        }
    }
}
