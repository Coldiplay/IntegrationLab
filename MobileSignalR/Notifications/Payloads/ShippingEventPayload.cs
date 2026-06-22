using System.Text.Json.Serialization;
using BaseLibrary.Model.Classes;
using BaseLibrary.Model.Enums;

namespace MobileSignalR.Notifications.Payloads;

public class ShippingEventPayload : IEventPayload
{
    public ulong Id { get; set; }

    public string DeliveryPoint { get; set; } = null!;

    public DateTime EstimatedDeliveryDate { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public ShippingStatus ShippingStatus { get; set; }

    public DateOnly ShippingDate { get; set; }

    public DateTime? ShippedDate { get; set; }

    public ulong VehicleId { get; set; }

    public Guid DesignatedDriverId { get; set; }
    
    
    [JsonPropertyName("action")] public string Action { get; set; } = null!; // "created" | "updated" | "deleted"
    [JsonPropertyName("version")] public int? Version { get; set; }

    public static explicit operator Shipping(ShippingEventPayload payload) => new() {
         Id = payload.Id,
         DeliveryPoint = payload.DeliveryPoint,
         EstimatedDeliveryDate = payload.EstimatedDeliveryDate,
         DeliveryDate = payload.DeliveryDate,
         ShippingStatus = payload.ShippingStatus,
         VehicleId = payload.VehicleId,
         DesignatedDriverId = payload.DesignatedDriverId,
         ShippedDate = payload.ShippedDate,
         ShippingDate = payload.ShippingDate,
    };
}