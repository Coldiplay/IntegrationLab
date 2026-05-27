using System.Text.Json.Serialization;

namespace MobileSignalR.NotificationHandlers.Events;

internal sealed class MessageEvent
{
    [JsonPropertyName("envelope")] public Envelope Envelope { get; set; } = null!;
    [JsonPropertyName("payload")] public MessagePayload Payload { get; set; } = null!;
}