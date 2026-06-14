using MobileSignalR.Notifications.Payloads;
using Newtonsoft.Json;

namespace MobileSignalR.Notifications.Events;

public abstract class BaseEventHandler<TPayload> : IEventHandler where TPayload : IEventPayload
{
    public abstract string EventType { get; }  // например "MessageCreated"

    public virtual bool CanHandle(string eventType) =>
        string.Equals(eventType, EventType, StringComparison.OrdinalIgnoreCase);

    public async Task HandleAsync(EventEnvelope envelope, string payload, CancellationToken ct)
    {
        var typedPayload = JsonConvert.DeserializeObject<TPayload>(payload, RabbitMqConsumerOptions.JsonSettings);
        if (typedPayload is null)
            throw new InvalidOperationException($"Failed to deserialize payload to {typeof(TPayload).Name}");
        
        await HandleCoreAsync(envelope, typedPayload, ct);
    }

    protected abstract Task HandleCoreAsync(EventEnvelope envelope, TPayload payload, CancellationToken ct);
}