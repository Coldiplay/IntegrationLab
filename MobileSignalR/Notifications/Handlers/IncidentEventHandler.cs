using BaseLibrary.Model.Classes;
using Microsoft.AspNetCore.SignalR;
using MobileSignalR.Hub;
using MobileSignalR.Notifications.Events;
using MobileSignalR.Notifications.Payloads;

namespace MobileSignalR.Notifications.Handlers;

public class IncidentEventHandler(
    IHubContext<MobileHub> hubContext,
    ILogger<IncidentEventHandler> logger) 
    : BaseEventHandler<IncidentEventPayload>
{
    public override string EventType => "Incident";
    public override bool CanHandle(string eventType) =>
        eventType.StartsWith(EventType, StringComparison.OrdinalIgnoreCase);

    protected override async Task HandleCoreAsync(EventEnvelope envelope, IncidentEventPayload payload, CancellationToken ct)
    {
        (string hubMethod, object signalrPayload) tuple = payload.Action.ToLowerInvariant() switch {
            "created" => ("IncidentReceive", (Incident)payload),
            "updated" => ("IncidentDeleted", (Incident)payload),
            "deleted" => ("IncidentDeleted", new{ payload.Id}),
            _ => ("IncidentUpdated", (Incident)payload)
        };

        var groupName = "Incident " + payload.Id;
        
        await hubContext.Clients.Group(groupName).SendAsync(tuple.hubMethod, tuple.signalrPayload, CancellationToken.None);
        
        logger.LogInformation("Incident({IncidentId}) event {EventType} delivered", payload.Id, envelope.Type);
    }
}