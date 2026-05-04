namespace BaseLibrary.TestDB;

public partial class Shipping
{
    public ulong Id { get; set; }

    public string DeliveryPoint { get; set; } = null!;

    public DateTime EstimatedDeliveryDate { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public string ShippingStatus { get; set; } = null!;

    public DateOnly ShippingDate { get; set; }

    public DateTime? ShippedDate { get; set; }

    public ulong VehicleId { get; set; }

    public ulong DesignatedDriverId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
