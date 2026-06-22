using System.Text.Json.Serialization;
using BaseLibrary.Model.Classes;

namespace MobileSignalR.Notifications.Payloads;

public class ChatEventPayload : IEventPayload
{
    public ulong Id { get; set; }
    public string? Name { get; set; }
    public bool? IsPrivateChat { get; set; }
    
    //TODO: Добавить chatmembers для created и updated
    
    [JsonPropertyName("action")] public string Action { get; set; } = null!; // "created" | "updated" | "deleted"
    [JsonPropertyName("version")] public int? Version { get; set; }


    public static explicit operator Chat(ChatEventPayload payload) => new() { 
        Id = payload.Id, 
        Name = payload.Name,
        IsPrivateChat = payload.IsPrivateChat,
    };
}