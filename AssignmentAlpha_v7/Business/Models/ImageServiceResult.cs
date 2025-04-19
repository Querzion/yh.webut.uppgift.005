using Domain.Models;

namespace Business.Models;

public interface IImageServiceResult
{
    IEnumerable<Image>? Result { get; set; }
}

public class ImageServiceResult : ServiceResult, IImageServiceResult
{
    public IEnumerable<Image>? Result { get; set; }
}