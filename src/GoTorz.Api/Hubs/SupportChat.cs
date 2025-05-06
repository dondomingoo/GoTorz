using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

/// <summary>
/// Hub for handling real-time support chat between a user and the server.
/// Each user is automatically placed into a private group based on their username.
/// Admins can join and interact with user groups.
/// </summary>
[Authorize]
public class SupportChatHub : Hub
{
    /// <summary>
    /// Called automatically when a client connects to the hub.
    /// Each user is placed into a private group based on their username.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var username = Context.User?.Identity?.Name;
        Console.WriteLine($"[OnConnectedAsync] Connected as: '{username}'");

        if (!string.IsNullOrWhiteSpace(username))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"private-{username}");
            Console.WriteLine($"[OnConnectedAsync] Added to group: private-{username}");
        }
        else
        {
            Console.WriteLine($"[OnConnectedAsync] No username found! Connection won't be added to group.");
        }

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called by a normal user to send a message to their own private group.
    /// </summary>
    /// <param name="message">The message content to send.</param>
    public async Task SendUserMessage(string message)
    {
        var username = Context.User?.Identity?.Name;
        Console.WriteLine($"[SendUserMessage] Sender username: '{username}'");
        Console.WriteLine($"[SendUserMessage] Sending message: '{message}'");

        if (!string.IsNullOrWhiteSpace(username) && !Context.User.IsInRole("Admin"))
        {
            await Clients.Group($"private-{username}").SendAsync("ReceiveMessage", username, message);
            Console.WriteLine($"[SendUserMessage] Sent to group: private-{username}");
        }
        else
        {
            Console.WriteLine($"[SendUserMessage] Message blocked. Either no username or sender is admin.");
        }
    }

    /// <summary>
    /// Allows an admin to join a private support room for a specific user.
    /// Only users with the "Admin" role are allowed to use this.
    /// </summary>
    /// <param name="targetUser">The username (email) of the user to assist.</param>
    [Authorize(Roles = "Admin")]
    public async Task JoinSupportRoom(string targetUser)
    {
        var adminName = Context.User?.Identity?.Name;

        if (!Context.User.IsInRole("Admin"))
        {
            Console.WriteLine($"[JoinSupportRoom] Access denied. '{adminName}' is not an admin.");
            return;
        }

        if (string.IsNullOrWhiteSpace(targetUser))
        {
            Console.WriteLine($"[JoinSupportRoom] Invalid target user.");
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"private-{targetUser}");
        Console.WriteLine($"[JoinSupportRoom] Admin '{adminName}' joined private-{targetUser}");
    }

    /// <summary>
    /// Called by admins to send a message to a user's private group.
    /// </summary>
    /// <param name="targetUser">The username (email) of the user to send the message to.</param>
    /// <param name="message">The message content to send.</param>
    [Authorize(Roles = "Admin")]
    public async Task SendAdminMessage(string targetUser, string message)
    {
        var adminName = Context.User?.Identity?.Name;
        Console.WriteLine($"[SendAdminMessage] Admin '{adminName}' sends: '{message}' to '{targetUser}'");

        if (!Context.User.IsInRole("Admin"))
        {
            Console.WriteLine($"[SendAdminMessage] Access denied. '{adminName}' is not an admin.");
            return;
        }

        if (string.IsNullOrWhiteSpace(targetUser))
        {
            Console.WriteLine($"[SendAdminMessage] Invalid target user.");
            return;
        }

        await Clients.Group($"private-{targetUser}").SendAsync("ReceiveMessage", $"Admin ({adminName})", message);
    }
}