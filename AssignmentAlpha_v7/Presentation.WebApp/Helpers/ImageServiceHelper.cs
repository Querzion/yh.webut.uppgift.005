using Business.Models;
using Business.Services;
using Domain.DTOs.Forms;
using Microsoft.AspNetCore.Http;

namespace Presentation.WebApp.Helpers;

public interface IImageServiceHelper
{
    Task<ImageServiceResult> SaveImageAsync(IFormFile file, string purpose, ImageFormData metadata);
}

public class ImageServiceHelper(IWebHostEnvironment env, IImageService imageService) : IImageServiceHelper
{
    private readonly IImageService _imageService = imageService;
    private readonly IWebHostEnvironment _env = env;
    
    public async Task<ImageServiceResult> SaveImageAsync(IFormFile file, string purpose, ImageFormData metadata)
    {
        // Validate if the file is empty or null
        if (file == null || file.Length == 0)
            return new ImageServiceResult { Succeeded = false, StatusCode = 400, Error = "No file provided or file is empty." };

        var uploadFolder = Path.Combine(_env.WebRootPath, "uploads", purpose);
        Directory.CreateDirectory(uploadFolder);

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"[{DateTime.UtcNow:yyyy-MM-dd}].[{Guid.NewGuid()}]{extension}";
        var savePath = Path.Combine(uploadFolder, fileName);

        // Save the file asynchronously
        try
        {
            await using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }
        catch (Exception ex)
        {
            return new ImageServiceResult { Succeeded = false, StatusCode = 500, Error = $"Failed to save file. {ex.Message}" };
        }

        // Generate the relative image URL
        var imageUrl = Path.Combine("/uploads", purpose, fileName).Replace("\\", "/");

        // Update the metadata's ImageUrl
        metadata.ImageUrl = imageUrl;

        // Send the image metadata to the ImageService for processing
        var imageResult = await _imageService.ProcessImageAsync(metadata);

        // Handle the result from ProcessImageAsync
        if (!imageResult.Succeeded)
        {
            return new ImageServiceResult
            {
                Succeeded = false,
                StatusCode = imageResult.StatusCode,
                Error = imageResult.Error
            };
        }

        // If the image processing was successful, return the result
        return new ImageServiceResult
        {
            Succeeded = true,
            StatusCode = 201,
            Result = imageResult.Result // Optionally wrap it if needed
        };
    }
}