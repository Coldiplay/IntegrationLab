using BaseLibrary.Model.Classes;
using Microsoft.AspNetCore.SignalR;
using MobileSignalR.Hub;
using MobileSignalR.Notifications.Events;
using MobileSignalR.Notifications.Payloads;

namespace MobileSignalR.Notifications.Handlers;

public class ShippingEventHandler(
    IHubContext<MobileHub> hubContext,
    ILogger<ShippingEventPayload> logger) 
    : BaseEventHandler<ShippingEventPayload>
{
    public override string EventType => "Shipping";
    public override bool CanHandle(string eventType) =>
        eventType.StartsWith(EventType, StringComparison.OrdinalIgnoreCase);

    protected override async Task HandleCoreAsync(EventEnvelope envelope, ShippingEventPayload payload, CancellationToken ct)
    {
        (string hubMethod, object signalrPayload) tuple = payload.Action.ToLowerInvariant() switch {
            "created" => ("ShippingReceive",  (Shipping)payload),
            "updated" => ("ShippingUpdated", (Shipping)payload),
            "deleted" => ("ShippingDeleted", new{ payload.Id}),
            _ => ("Shipping", (Shipping)payload)
        };

        var groupName = "Shipping " + payload.Id;
        
        await hubContext.Clients.Group(groupName).SendAsync(tuple.hubMethod, tuple.signalrPayload, CancellationToken.None);
        logger.LogInformation("Shipping({ShippingId}) event {EventType} delivered", payload.Id, envelope.Type);
    }
}