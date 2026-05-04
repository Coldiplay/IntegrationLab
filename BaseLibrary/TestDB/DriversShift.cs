namespace BaseLibrary.TestDB;

public partial class DriversShift
{
    public ulong Id { get; set; }

    public DateTime Start { get; set; }

    public DateTime? End { get; set; }

    public ulong DriverId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
