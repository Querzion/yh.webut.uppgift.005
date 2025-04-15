using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Domain.Models;

namespace Data.Repositories;

public class ImageRepository(AppDbContext context) : BaseRepository<ImageEntity, Image>(context), IImageRepository
{
    
}