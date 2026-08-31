namespace WEB_453504_ASP_NET.UI.Services.FileSevice;


public class LocalFileService : IFileService
{
    private readonly string _fileStoragePath;
    private readonly IWebHostEnvironment env;
    private readonly ILogger<LocalFileService> logger;
    public LocalFileService(IConfiguration configuration, IWebHostEnvironment env, ILogger<LocalFileService> logger)
    {
        _fileStoragePath = configuration.GetValue<string>("FileStoragePath") ?? "images/avatars";
        this.env = env;
        this.logger = logger;
    }

    public async Task<string> SaveFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty or null", nameof(file));

        var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
        var filePath = $"{_fileStoragePath}/{fileName}";

        var serverPath = $"{env.WebRootPath}/{_fileStoragePath}/{fileName}";
        logger.LogInformation($"-----> Saving file to: {serverPath}");
        Directory.CreateDirectory(Path.GetDirectoryName(serverPath));

        using (var stream = new FileStream(serverPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        return filePath;
    }
}