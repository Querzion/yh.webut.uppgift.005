using Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public interface ITagsService
{
    Task<IEnumerable<string>> SearchTagsAsync(string term);
}

public class TagsService(AppDbContext context) : ITagsService
{
    public async Task<IEnumerable<string>> SearchTagsAsync(string term)
    {
        if (string.IsNullOrWhiteSpace(term)) return new List<string>();

        return await context.Tags
            .Where(x => x.TagName.Contains(term))
            .Select(x => x.TagName)  // Assuming you want just tag names
            .ToListAsync();
    }
}