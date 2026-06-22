using System.Text.Json.Serialization;
using BaseLibrary.Model.Classes;

namespace MobileSignalR.Notifications.Payloads;

public sealed class MessagePayload : IEventPayload
{
    [JsonPropertyName("id")] public ulong Id { get; set; }
    [JsonPropertyName("chat_id")] public ulong ChatId { get; set; }
    [JsonPropertyName("user_id")] public string UserId { get; set; } = null!;
    [JsonPropertyName("content")] public string? Content { get; set; }
    [JsonPropertyName("created_at")] public DateTimeOffset? CreatedAt { get; set; }
    [JsonPropertyName("updated_at")] public DateTimeOffset? UpdatedAt { get; set; }
    [JsonPropertyName("action")] public string Action { get; set; } = null!; // "created" | "updated" | "deleted"
    [JsonPropertyName("version")] public int? Version { get; set; }


    public static explicit operator Message(MessagePayload payload) => new() {
        Id = payload.Id,
        ChatId = payload.ChatId,
        SenderId = Guid.Parse(payload.UserId),
        Content = payload.Content,
        CreatedAt = payload.CreatedAt?.DateTime,
        UpdatedAt = payload.UpdatedAt?.DateTime
    };
}