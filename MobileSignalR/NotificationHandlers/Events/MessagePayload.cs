using System.Text.Json.Serialization;

namespace MobileSignalR.NotificationHandlers.Events;

internal sealed class MessagePayload
{
    [JsonPropertyName("id")] public string Id { get; set; } = null!;
    [JsonPropertyName("chat_id")] public string ChatId { get; set; } = null!;
    [JsonPropertyName("user_id")] public string UserId { get; set; } = null!;
    [JsonPropertyName("action")] public string Action { get; set; } = null!; // "created" | "updated" | "deleted"
    [JsonPropertyName("content")] public string? Content { get; set; }
    [JsonPropertyName("updated_at")] public DateTimeOffset? UpdatedAt { get; set; }
    [JsonPropertyName("version")] public int? Version { get; set; }
}