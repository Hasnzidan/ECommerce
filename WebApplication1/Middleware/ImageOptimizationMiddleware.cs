using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace WebApplication1.Middleware
{
    public class ImageOptimizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _imageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

        public ImageOptimizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;

            // Check if the request is for an image
            if (!string.IsNullOrEmpty(path) && 
                _imageExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            {
                // Check if size parameter is present
                if (int.TryParse(context.Request.Query["size"].ToString(), out int size))
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path.TrimStart('/'));
                    if (File.Exists(imagePath))
                    {
                        // Create cache key based on path and size
                        var cacheKey = $"{path}_{size}";
                        var cacheFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "cache");
                        var cachedImagePath = Path.Combine(cacheFolder, cacheKey);

                        // Check if cached version exists
                        if (!File.Exists(cachedImagePath))
                        {
                            // Ensure cache directory exists
                            Directory.CreateDirectory(cacheFolder);

                            // Process and cache the image
                            using (var image = await Image.LoadAsync(imagePath))
                            {
                                var ratio = (double)size / Math.Min(image.Width, image.Height);
                                var newWidth = (int)(image.Width * ratio);
                                var newHeight = (int)(image.Height * ratio);

                                image.Mutate(x => x
                                    .Resize(newWidth, newHeight)
                                    .AutoOrient());

                                await image.SaveAsync(cachedImagePath);
                            }
                        }

                        // Serve the cached image
                        await context.Response.SendFileAsync(cachedImagePath);
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
