namespace MobileSignalR.Notifications.Payloads;

public interface IEventPayload
{
    string Action { get; }
    int? Version { get; }
}