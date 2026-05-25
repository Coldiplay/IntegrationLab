using Microsoft.AspNetCore.SignalR;

namespace MobileSignalR.Hub;

public class CustomUserId : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value;
    }
}