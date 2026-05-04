namespace BaseLibrary.TestDB;

public partial class Cargo
{
    public ulong Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public double Weight { get; set; }

    public double DimensionsLength { get; set; }

    public double DimensionsHeight { get; set; }

    public double DimensionsWidth { get; set; }

    public string? DangerLevel { get; set; }

    public ulong ShippingOrderId { get; set; }

    public ulong? ShippingId { get; set; }

    public ulong CargoTypeId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
