using BaseLibrary.Model.Classes;
using Microsoft.AspNetCore.SignalR;
using MobileSignalR.Hub;
using MobileSignalR.Notifications.Events;
using MobileSignalR.Notifications.Payloads;
using MobileSignalR.Tools;

namespace MobileSignalR.Notifications.Handlers;

public class MessageEventHandler(
    IHubContext<MobileHub> hubContext,
    ConnectionsHandler connections,
    ILogger<MessageEventHandler> logger)
    : BaseEventHandler<MessagePayload>
{
    public override string EventType => "Message";
    
    // Принимаем MessageCreated, MessageUpdated, MessageDeleted
    public override bool CanHandle(string eventType) =>
        eventType.StartsWith("Message", StringComparison.OrdinalIgnoreCase);

    protected override async Task HandleCoreAsync(EventEnvelope envelope, MessagePayload payload, CancellationToken ct)
    {
        (string hubMethod, object signalrPayload) tuple = payload.Action.ToLowerInvariant() switch {
            "created" => ("ReceiveMessage", new Message {
                Id = ulong.Parse(payload.Id),
                ChatId = ulong.Parse(payload.ChatId),
                Content = payload.Content,
                SenderId = Guid.Parse(payload.UserId),
                CreatedAt = payload.UpdatedAt?.DateTime
            }),
            "updated" => ("MessageUpdated", new Message {
                Id = ulong.Parse(payload.Id),
                ChatId = ulong.Parse(payload.ChatId),
                Content = payload.Content,
                UpdatedAt = payload.UpdatedAt?.DateTime
            }),
            "deleted" => ("MessageDeleted", new{ payload.Id, payload.ChatId}),
            _ => ("MessageUpdated", new Message {
                Id = ulong.Parse(payload.Id),
                ChatId = ulong.Parse(payload.ChatId),
                Content = payload.Content,
                UpdatedAt = payload.UpdatedAt?.DateTime
            })
        };
        var groupName = "Chat " + payload.ChatId; 

        // Находим всех участников чата, кроме отправителя (userId)
        var senderId = Guid.Parse(payload.UserId);
        var senderConnections = connections.GetConnections(senderId);

        if (senderConnections is not null)
        {
            await hubContext.Clients.GroupExcept(groupName, senderConnections)
                .SendAsync(tuple.hubMethod, tuple.signalrPayload, ct);
        }
        
        logger.LogInformation("Message event {EventType} delivered to chat {ChatId}",
            envelope.Type, payload.ChatId);
    }
}