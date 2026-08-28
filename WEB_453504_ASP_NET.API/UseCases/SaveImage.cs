using MediatR;

namespace WEB_453504_ASP_NET.API.UseCases
{
    public sealed record SaveImage(IFormFile file) : IRequest<string>;

    public class SaveImageHandler(IWebHostEnvironment env) : IRequestHandler<SaveImage, string>
    {
        public async Task<string> Handle(SaveImage request, CancellationToken cancellationToken)
        {
            try
            {
                var webRoot = env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var imagesFolder = Path.Combine(webRoot, "images");

                if (!Directory.Exists(imagesFolder))
                    Directory.CreateDirectory(imagesFolder);

                var fileExtension = Path.GetExtension(request.file.FileName);
                var newFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(imagesFolder, newFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.file.CopyToAsync(stream, cancellationToken);
                }

                return newFileName;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка при сохранении файла: {ex.Message}", ex);
            }
        }
    }
}
