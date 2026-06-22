using BaseLibrary.Model.Classes;
using Microsoft.AspNetCore.SignalR;
using MobileSignalR.Hub;
using MobileSignalR.Notifications.Events;
using MobileSignalR.Notifications.Payloads;

namespace MobileSignalR.Notifications.Handlers;

public class ChatEventHandler(
    IHubContext<MobileHub> hubContext,
    ILogger<ChatEventHandler> logger) 
    : BaseEventHandler<ChatEventPayload>
{
    public override string EventType => "Chat";
    public override bool CanHandle(string eventType) =>
        eventType.StartsWith(EventType, StringComparison.OrdinalIgnoreCase);

    protected override async Task HandleCoreAsync(EventEnvelope envelope, ChatEventPayload payload, CancellationToken ct)
    {
        (string hubMethod, object signalrPayload) tuple = payload.Action.ToLowerInvariant() switch {
            "created" => ("ChatReceive", (Chat)payload),
            "updated" => ("ChatDeleted", (Chat)payload),
            "deleted" => ("ChatDeleted", new{ payload.Id}),
            _ => ("ChatUpdated", (Chat)payload)
        };

        var groupName = "Chat " + payload.Id;
        
        await hubContext.Clients.Group(groupName).SendAsync(tuple.hubMethod, tuple.signalrPayload, CancellationToken.None);
        logger.LogInformation("Chat({ChatId}) event {EventType} delivered", payload.Id, envelope.Type);
    }
}