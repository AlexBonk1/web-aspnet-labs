using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.StaticFiles;

namespace WEB_453504_ASP_NET.API.Endpoints
{
    public static class ImagesEndpoints
    {
        public static void MapImages(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/images");

            group.MapGet("/{filename}", async (string fileName, IWebHostEnvironment env) =>
            {
                var webRoot = env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var imagePath = Path.Combine(webRoot, "images", fileName);

                if (!File.Exists(imagePath))
                    return Results.NotFound();

                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(imagePath, out var contentType))
                    contentType = "application/octet-stream";

                var bytes = await File.ReadAllBytesAsync(imagePath);
                return Results.File(bytes, contentType);
            })
            .WithName("GetImage");

        }

    }
}
