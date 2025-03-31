using Microsoft.AspNetCore.SignalR;

/// <summary>
/// Hub for handling real-time support chat between a user and the server.
/// Each user is automatically placed into a private group based on their username.
/// </summary>
public class SupportChatHub : Hub
{

    /// <summary>
    /// Called by the client to send a message.
    /// The message will be sent to the sender's private group.
    /// </summary>
    /// <param name="message">The message content to send.</param>
    public async Task SendMessage(string message)
    {
        var username = Context.User?.Identity?.Name;
        Console.WriteLine($"[SendMessage] Sender username: '{username}'"); // Debug logs - useful during development                                        
        Console.WriteLine($"[SendMessage] Sending message: '{message}'");                                       

        if (!string.IsNullOrWhiteSpace(username))
        {
            await Clients.Group($"private-{username}").SendAsync("ReceiveMessage", username, message);
            Console.WriteLine($"[SendMessage] Sent to group: private-{username}");                              
        }
        else
        {
            Console.WriteLine($"[SendMessage] No username found, cannot send message.");                        
        }
    }

    /// <summary>
    /// Automatically called when a client connects.
    /// Adds the client to a private group based on their username.
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

}

