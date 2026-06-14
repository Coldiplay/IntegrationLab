using System.Text.Json.Serialization;
using MobileSignalR.Notifications.Payloads;

namespace MobileSignalR.Notifications.Events;

internal sealed class MessageEvent
{
    [JsonPropertyName("envelope")] public EventEnvelope EventEnvelope { get; set; } = null!;
    [JsonPropertyName("payload")] public MessagePayload Payload { get; set; } = null!;
}