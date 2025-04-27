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
    Task<ImageServiceResult> SaveImageAsync(ImageFormData formData);
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

    public async Task<ImageServiceResult> SaveImageAsync(ImageFormData formData)
    {
        var result = new ImageServiceResult();

        try
        {
            if (formData == null)
            {
                result.Succeeded = false;
                result.Error = "Image form data is null.";
                result.StatusCode = 400;
                return result;
            }

            // 1. Create ImageEntity from ImageFormData
            var newImageEntity = new ImageEntity
            {
                ImageUrl = formData.ImageUrl,
                AltText = formData.AltText
            };

            // 2. Save the ImageEntity into the database
            await _imageRepository.AddAsync(newImageEntity);
            await _imageRepository.SaveChangesAsync();

            // 3. Map to Image model (or any other return type)
            var mappedImage = newImageEntity.MapTo<Image>();

            result.Succeeded = true;
            result.Result = new List<Image> { mappedImage };
            result.StatusCode = 200;
        }
        catch (Exception ex)
        {
            result.Succeeded = false;
            result.Error = ex.Message;
            result.StatusCode = 500;
        }

        return result;
    }
    
    public async Task<ImageServiceResult> DeleteImageAsync(string imageId)
    {
        var result = new ImageServiceResult();

        // Try to find the image using the ImageId
        var imageResult = await _imageRepository.GetEntityAsync(
            x => x.Id == imageId
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
        result.Result = new List<Image> { imageEntity.MapTo<Image>() }; // Map to a domain model if needed
        return result;
    }
}