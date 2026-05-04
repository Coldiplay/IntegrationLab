namespace BaseLibrary.TestDB;

public partial class Incident
{
    public ulong Id { get; set; }

    public ulong ShippingId { get; set; }

    public ulong DriverId { get; set; }

    public string? Description { get; set; }

    public DateTime IncidentDate { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
