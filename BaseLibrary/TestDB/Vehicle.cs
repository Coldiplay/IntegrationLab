namespace BaseLibrary.TestDB;

public partial class Vehicle
{
    public ulong Id { get; set; }

    public string VehicleNumberPlate { get; set; } = null!;

    public string Brand { get; set; } = null!;

    public string Model { get; set; } = null!;

    public uint NeededRights { get; set; }

    public double LiftingCapacity { get; set; }

    public string BodyType { get; set; } = null!;

    public double VehicleSizeLength { get; set; }

    public double VehicleSizeWidth { get; set; }

    public double VehicleSizeHeight { get; set; }

    public double BodySizeLength { get; set; }

    public double BodySizeWidth { get; set; }

    public double BodySizeHeight { get; set; }

    public double MaxCargoVolume { get; set; }

    public double VehicleWeight { get; set; }

    public byte NumberOfAxes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
