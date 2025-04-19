using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Hubs;

public class NotificationHub : Hub
{
    // Send a notification to all clients
    public async Task SendNotificationToAll(object notification)
    {
        await Clients.All.SendAsync("AllReceiveNotification", notification);
    }
    
    // Send a notification to all admins
    public async Task SendNotificationToAdmins(object notification)
    {
        // Use Groups to send notifications only to users in the "admins" group
        await Clients.Group("admins").SendAsync("AdminReceiveNotification", notification);
    }
    
    // Send a notification to all managers
    public async Task SendNotificationToManagers(object notification)
    {
        // Use Groups to send notifications only to users in the "managers" group
        await Clients.Group("managers").SendAsync("ManagerReceiveNotification", notification);
    }

    // Add a user to a specific group (e.g., admins or managers)
    public async Task AddToAdminGroup()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "admins");
    }

    public async Task AddToManagerGroup()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "managers");
    }

    // Remove a user from a specific group (e.g., admins or managers)
    public async Task RemoveFromAdminGroup()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "admins");
    }

    public async Task RemoveFromManagerGroup()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "managers");
    }
}