using Microsoft.AspNetCore.Http;

namespace Domain.Handlers;

public interface IFileHandler
{
    Task<string> UploadFileAsync(IFormFile file);
}