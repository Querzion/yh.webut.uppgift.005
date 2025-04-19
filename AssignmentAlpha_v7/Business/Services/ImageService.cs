using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Domain.DTOs.Forms;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Business.Services;

public interface IImageService
{
    Task<ImageServiceResult> ProcessImageAsync(ImageFormData metadata);
    Task<ImageServiceResult> DeleteImageAsync(string imageId);
}

public class ImageService(IImageRepository imageRepository) : IImageService
{
    private readonly IImageRepository _imageRepository = imageRepository;

    public async Task<ImageServiceResult> ProcessImageAsync(ImageFormData metadata)
    {
        try
        {
            // Ensure the required metadata values are present
            if (string.IsNullOrEmpty(metadata.ImageUrl) || string.IsNullOrEmpty(metadata.AltText))
            {
                return new ImageServiceResult
                {
                    Succeeded = false,
                    StatusCode = 400,
                    Error = "ImageUrl and AltText are required."
                };
            }

            // Create the image entity from the metadata
            var imageEntity = new ImageEntity
            {
                Id = Guid.NewGuid().ToString(),
                ImageUrl = metadata.ImageUrl,
                AltText = metadata.AltText,
                UploadedAt = DateTime.UtcNow
            };

            // Attempt to save the image entity to the repository
            var saveResult = await _imageRepository.AddAsync(imageEntity);

            if (!saveResult.Succeeded)
            {
                return new ImageServiceResult
                {
                    Succeeded = false,
                    StatusCode = 500,
                    Error = saveResult.Error
                };
            }

            // Return the result wrapped in an ImageServiceResult
            return new ImageServiceResult
            {
                Succeeded = true,
                StatusCode = 201,
                Result = new List<Image> { saveResult.Result.MapTo<Image>() } // Wrap the single result in a list for consistency
            };
        }
        catch (Exception ex)
        {
            // Handle unexpected errors
            return new ImageServiceResult
            {
                Succeeded = false,
                StatusCode = 500,
                Error = ex.Message
            };
        }
    }
    
    public async Task<ImageServiceResult> DeleteImageAsync(string imageUrl)
    {
        var result = new ImageServiceResult();

        // Try to find the image
        var imageResult = await _imageRepository.GetEntityAsync(
            x => x.ImageUrl == imageUrl
        );

        if (!imageResult.Succeeded || imageResult.Result == null)
        {
            result.Succeeded = false;
            result.StatusCode = 404;
            result.Error = "Image not found.";
            return result;
        }

        var imageEntity = imageResult.Result;

        // Proceed with deletion
        var deleteResult = await _imageRepository.DeleteAsync(imageEntity);
        if (!deleteResult.Succeeded)
        {
            result.Succeeded = false;
            result.StatusCode = 500;
            result.Error = "Failed to delete image.";
            return result;
        }

        // Return success with the deleted image in Result
        result.Succeeded = true;
        result.StatusCode = 200;
        result.Result = new List<Image> { imageEntity.MapTo<Image>() };
        return result;
    }
}