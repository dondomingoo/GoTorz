using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

/// <summary>
/// Hub for handling real-time support chat between a user and the server.
/// Each user is automatically placed into a private group based on their username.
/// Admins and SalesReps can join and interact with user groups.
/// </summary>
[Authorize]
public class SupportChatHub : Hub
{
    private static readonly HashSet<string> _connectedUsernames = new();
    private static readonly object _lock = new();

    /// <summary>
    /// Called automatically when a client connects to the hub.
    /// Each regular user is placed into a private group based on their username.
    /// Support agents (Admin, SalesRep) are not.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var username = Context.User?.Identity?.Name;
        var isSupportAgent = IsSupportAgent(Context.User);

        Console.WriteLine($"[OnConnectedAsync] Connected as: '{username}'");

        if (!string.IsNullOrWhiteSpace(username) && !isSupportAgent)
        {
            lock (_lock)
            {
                _connectedUsernames.Add(username);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, $"private-{username}");
            Console.WriteLine($"[OnConnectedAsync] User '{username}' added to group.");
        }
        else
        {
            Console.WriteLine($"[OnConnectedAsync] Support agent or no username. Not added to group.");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var username = Context.User?.Identity?.Name;
        var isSupportAgent = IsSupportAgent(Context.User);

        if (!string.IsNullOrWhiteSpace(username) && !isSupportAgent)
        {
            lock (_lock)
            {
                _connectedUsernames.Remove(username);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Support agents (Admin or SalesRep) can see who is currently connected.
    /// </summary>
    [Authorize(Roles = "Admin,SalesRep")]
    public Task<List<string>> GetConnectedUsers()
    {
        lock (_lock)
        {
            return Task.FromResult(_connectedUsernames.ToList());
        }
    }

    /// <summary>
    /// Called by a regular user to send a message to their own private group.
    /// </summary>
    /// <param name="message">The message content to send.</param>
    public async Task SendUserMessage(string message)
    {
        var username = Context.User?.Identity?.Name;
        Console.WriteLine($"[SendUserMessage] Sender username: '{username}'");
        Console.WriteLine($"[SendUserMessage] Sending message: '{message}'");

        if (!string.IsNullOrWhiteSpace(username) && !IsSupportAgent(Context.User))
        {
            await Clients.Group($"private-{username}").SendAsync("ReceiveMessage", username, message);
            Console.WriteLine($"[SendUserMessage] Sent to group: private-{username}");
        }
        else
        {
            Console.WriteLine($"[SendUserMessage] Message blocked. Sender is support agent or username is missing.");
        }
    }

    /// <summary>
    /// Support agents (Admin or SalesRep) can join a user's private support room.
    /// </summary>
    /// <param name="targetUser">The username (email) of the user to assist.</param>
    [Authorize(Roles = "Admin,SalesRep")]
    public async Task JoinSupportRoom(string targetUser)
    {
        var supportName = Context.User?.Identity?.Name;

        if (!IsSupportAgent(Context.User))
        {
            Console.WriteLine($"[JoinSupportRoom] Access denied. '{supportName}' is not a support agent.");
            return;
        }

        if (string.IsNullOrWhiteSpace(targetUser))
        {
            Console.WriteLine($"[JoinSupportRoom] Invalid target user.");
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"private-{targetUser}");
        Console.WriteLine($"[JoinSupportRoom] Support agent '{supportName}' joined private-{targetUser}");
    }

    /// <summary>
    /// Support agents (Admin or SalesRep) send a message to a user's private group.
    /// </summary>
    /// <param name="targetUser">The username (email) of the user to send the message to.</param>
    /// <param name="message">The message content to send.</param>
    [Authorize(Roles = "Admin,SalesRep")]
    public async Task SendAdminMessage(string targetUser, string message)
    {
        var supportName = Context.User?.Identity?.Name;

        if (!IsSupportAgent(Context.User))
        {
            Console.WriteLine($"[SendAdminMessage] Access denied. '{supportName}' is not a support agent.");
            return;
        }

        if (string.IsNullOrWhiteSpace(targetUser))
        {
            Console.WriteLine($"[SendAdminMessage] Invalid target user.");
            return;
        }

        string role = Context.User?.IsInRole("Admin") == true ? "Admin" : "Support";

        await Clients.Group($"private-{targetUser}")
            .SendAsync("ReceiveMessage", $"{role} ({supportName})", message);

        Console.WriteLine($"[SendAdminMessage] {role} '{supportName}' sent message to '{targetUser}'");
    }

    /// <summary>
    /// Determines if the current user is a support agent.
    /// </summary>
    private bool IsSupportAgent(ClaimsPrincipal? user)
    {
        return user?.IsInRole("Admin") == true || user?.IsInRole("SalesRep") == true;
    }
}
