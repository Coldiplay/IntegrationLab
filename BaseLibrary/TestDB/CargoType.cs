namespace BaseLibrary.TestDB;

public partial class CargoType
{
    public ulong Id { get; set; }

    public string? Title { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
