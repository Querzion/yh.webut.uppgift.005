using System.Security.Claims;
using Business.Services;
using Data.Entities;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Presentation.WebApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController(IHubContext<NotificationHub> notificationHub, INotificationService notificationService)
    : ControllerBase
{
    private readonly IHubContext<NotificationHub> _notificationHub = notificationHub;
    private readonly INotificationService _notificationService = notificationService;

    [HttpPost]
    public async Task<IActionResult> CreateNotificationAsync(NotificationEntity notificationEntity)
    {
        await _notificationService.AddNotificationAsync(notificationEntity);
        return Ok(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
        
        var notifications = await _notificationService.GetNotificationsAsync(userId);
        return Ok(notifications);
    }

    // [HttpPost("dismiss/{id}")]
    // public async Task<IActionResult> DismissNotification(string id)
    // {
    //     var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
    //     if (string.IsNullOrEmpty(userId))
    //         return Unauthorized();
    //     
    //     await _notificationService.DismissNotificationsAsync(id, userId);
    //     await _notificationHub.Clients.User(userId).SendAsync("NotificationDismissed", id);
    //     return Ok(new { success = true });
    // }
    
    [HttpPost("dismiss/{id}")]
    public async Task<IActionResult> DismissNotification(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
    
        try
        {
            await _notificationService.DismissNotificationsAsync(id, userId);
            await _notificationHub.Clients.User(userId).SendAsync("NotificationDismissed", id);
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }
    
    // [HttpDelete("{notificationId}")]
    // public async Task<IActionResult> DeleteNotification(string notificationId)
    // {
    //     try
    //     {
    //         await _notificationService.DeleteNotificationAsync(notificationId);
    //         return NoContent(); // Status code 204: Successfully deleted
    //     }
    //     catch (KeyNotFoundException)
    //     {
    //         return NotFound(); // Status code 404: Notification not found
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, ex.Message); // Status code 500: Internal Server Error
    //     }
    // }
}