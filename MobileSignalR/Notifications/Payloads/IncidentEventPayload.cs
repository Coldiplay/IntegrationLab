using System.Text.Json.Serialization;
using BaseLibrary.Model.Classes;
using BaseLibrary.Model.Enums;

namespace MobileSignalR.Notifications.Payloads;

public class IncidentEventPayload : IEventPayload
{
    public ulong Id { get; set; }

    public ulong ShippingId { get; set; }

    public Guid DriverId { get; set; }
    
    public string Description { get; set; } = null!;

    public DateTime IncidentDate { get; set; }

    public IncidentStatus Status { get; set; }
    
    
    [JsonPropertyName("action")] public string Action { get; set; } = null!; // "created" | "updated" | "deleted"
    [JsonPropertyName("version")] public int? Version { get; set; }

    public static explicit operator Incident(IncidentEventPayload payload) => new() {
        Id = payload.Id,
        DriverId = payload.DriverId,
        ShippingId = payload.ShippingId,
        Description = payload.Description,
        IncidentDate = payload.IncidentDate,
        Status = payload.Status,
    };
}