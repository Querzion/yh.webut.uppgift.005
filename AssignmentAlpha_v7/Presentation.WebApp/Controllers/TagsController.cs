using Business.Services;
using Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Presentation.WebApp.Controllers;

public class TagsController(ITagsService tagsService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> SearchTags(string term)
    {
        var tags = await tagsService.SearchTagsAsync(term);
        return Json(tags);
    }
}