namespace BaseLibrary.TestDB;

public partial class Driver
{
    public ulong UserId { get; set; }

    public uint Rights { get; set; }

    public string? DriversLicense { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
