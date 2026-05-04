namespace BaseLibrary.TestDB;

public partial class TransportCargoType
{
    public ulong VehicleId { get; set; }

    public ulong CargoTypeId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
