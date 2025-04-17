namespace Presentation.WebApp.Helpers;

public interface IImageUploadHelper
{
    Task<string?> UploadImageAsync(IFormFile file, string purpose);
}

public class ImageUploadHelper(IWebHostEnvironment env) : IImageUploadHelper
{
    public async Task<string?> UploadImageAsync(IFormFile file, string purpose)
    {
        if (file == null || file.Length == 0)
            return null;

        var uploadFolder = Path.Combine(env.WebRootPath, "uploads", purpose);
        Directory.CreateDirectory(uploadFolder);

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"[{DateTime.UtcNow:yyyy-MM-dd}].[{Guid.NewGuid()}]{extension}";
        var savePath = Path.Combine(uploadFolder, fileName);

        await using (var stream = new FileStream(savePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var imageUrl = Path.Combine("/uploads", purpose, fileName).Replace("\\", "/");
        return imageUrl;
    }
}