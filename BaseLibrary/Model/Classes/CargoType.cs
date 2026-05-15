namespace BaseLibrary.Model.Classes;

public partial class CargoType
{
    public ulong Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Cargo> Cargos { get; set; } = new List<Cargo>();

    public virtual ICollection<Vehicle> SupportedVehicles { get; set; } = new List<Vehicle>();
}
