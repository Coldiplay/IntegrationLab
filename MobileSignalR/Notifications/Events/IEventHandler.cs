namespace MobileSignalR.Notifications.Events;

public interface IEventHandler
{
    /// <summary>Проверяет, может ли обработчик обработать этот тип события</summary>
    bool CanHandle(string eventType);
    
    /// <summary>Выполняет обработку события. Получает общий конверт и сырой payload (json в string)</summary>
    Task HandleAsync(EventEnvelope envelope, string payload, CancellationToken ct);
}