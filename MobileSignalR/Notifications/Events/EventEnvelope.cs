using System.Text.Json.Serialization;

namespace MobileSignalR.Notifications.Events;

public sealed class EventEnvelope
{
    [JsonPropertyName("type")] public string Type { get; set; } = null!;
    [JsonPropertyName("version")] public string Version { get; set; } = "v1";
    [JsonPropertyName("eventId")] public string EventId { get; set; } = null!;
    [JsonPropertyName("occurredAt")] public DateTimeOffset OccurredAt { get; set; }
    [JsonPropertyName("correlationId")] public string? CorrelationId { get; set; }
    [JsonPropertyName("producer")] public string? Producer { get; set; }
}   