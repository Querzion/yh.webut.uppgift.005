using Data.Entities;
using Domain.Models;

namespace Data.Interfaces;

public interface IImageRepository : IBaseRepository<ImageEntity, Image>
{
    
}