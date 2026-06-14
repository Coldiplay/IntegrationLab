using MobileSignalR.Notifications.Events;

namespace MobileSignalR.Notifications.Handlers;

public class EventDispatcher(IEnumerable<IEventHandler> handlers, ILogger<EventDispatcher> logger)
{
    public async Task DispatchAsync(EventEnvelope envelope, string payload, CancellationToken ct)
    {
        var handler = handlers.FirstOrDefault(h => h.CanHandle(envelope.Type));
        if (handler == null)
        {
            logger.LogWarning("No handler registered for event type {EventType}", envelope.Type);
            // Можно dead-letter или игнорировать
            return;
        }

        await handler.HandleAsync(envelope, payload, ct);
    }
}