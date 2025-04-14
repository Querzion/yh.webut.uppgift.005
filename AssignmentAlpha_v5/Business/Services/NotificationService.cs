using Data.Contexts;
using Data.Entities;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public interface INotificationService
{
    Task AddNotificationAsync(NotificationEntity notificationEntity, string userId = "anonymous");
    Task<IEnumerable<NotificationEntity>> GetNotificationsAsync(string userId, int take = 10);
    Task DismissNotificationsAsync(string notificationId, string userId);
    Task DeleteNotificationAsync(string notificationId);
}

public class NotificationService(AppDbContext context, IHubContext<NotificationHub> notificationHub) : INotificationService
{
    private readonly AppDbContext _context = context;
    private readonly IHubContext<NotificationHub> _notificationHub = notificationHub;

    public async Task AddNotificationAsync(NotificationEntity notificationEntity, string userId = "anonymous")
    {
        if (string.IsNullOrEmpty(notificationEntity.Icon))
        {
            switch (notificationEntity.NotificationTypeId)
            {
                case 1 :
                    notificationEntity.Icon = "/images/profiles/user-template-male.svg";
                    break;
                case 2 :
                    notificationEntity.Icon = "/images/projects/project-template.svg";
                    break;
            }
        }

        _context.Add(notificationEntity);
        await _context.SaveChangesAsync();
        
        var notifications = await GetNotificationsAsync(userId);
        var newNotification = notifications.OrderByDescending(x => x.Created).FirstOrDefault();

        if (newNotification != null)
        {
            await _notificationHub.Clients.All.SendAsync("AllReceiveNotification", newNotification);
        }
    }

    public async Task<IEnumerable<NotificationEntity>> GetNotificationsAsync(string userId, int take = 10)
    {
        var dismissedIds = await _context.DismissedNotifications
            .Where(x => x.UserId == userId)
            .Select(x => x.NotificationId)
            .ToListAsync();

        var notifications = await _context.Notifications
            .Where(x => !dismissedIds.Contains(x.Id))
            .OrderByDescending(x => x.Created)
            .Take(take)
            .ToListAsync();

        return notifications;
    }

    public async Task DismissNotificationsAsync(string notificationId, string userId)
    {
        var alreadyDismissed = await _context.DismissedNotifications.AnyAsync(x => x.NotificationId == notificationId && x.UserId == userId);
        if (!alreadyDismissed)
        {
            var dismissed = new NotificationDismissedEntity
            {
                NotificationId = notificationId,
                UserId = userId
            };
            
            _context.Add(dismissed);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task DeleteNotificationAsync(string notificationId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(x => x.Id == notificationId);

        if (notification == null)
        {
            throw new KeyNotFoundException("Notification not found.");
        }

        // Optionally, you may want to remove related dismissed notifications
        var dismissedNotifications = await _context.DismissedNotifications
            .Where(x => x.NotificationId == notificationId)
            .ToListAsync();

        _context.DismissedNotifications.RemoveRange(dismissedNotifications);  // Remove related dismissed notifications

        _context.Notifications.Remove(notification);  // Remove the notification itself
        await _context.SaveChangesAsync();
    }
}